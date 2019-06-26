using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Regen.Compiler.Helpers;
using Regen.DataTypes;
using Regen.Exceptions;
using Regen.Helpers;
using Regen.Parser;
using Regen.Parser.Expressions;

namespace Regen.Compiler {
    public partial class RegenCompiler {
        /// <summary>
        ///     Compiles the given code and stores the variables in current <see cref="Context"/>.
        /// </summary>
        /// <param name="code"></param>
        public void CompileGlobal(string code) {
            //handle global blocks
            var parsed = ExpressionParser.Parse(code);
            Compile(parsed); //and we just ignore outputs, leaving all variables inside the context.
        }

        public string Compile(ParsedCode code) {
            var output = code.Output; //we cannot clone
            if (code.Variables != null) {
                foreach (var kv in code.Variables) {
                    Context.Variables[kv.Key] = kv.Value;
                }
            }

            foreach (var globalVariable in GlobalVariables)
                Context.Variables[globalVariable.Key] = globalVariable.Value;

            foreach (var action in code.ParseActions) {
                switch (action.Token) {
                    case ParserToken.Import: {
                        var expr = (ImportExpression) action.Related.Single();
                        var type = expr.Type;
                        var alias = expr.As;

                        if (File.Exists(type)) {
                            Assembly.LoadFile(type);
                            Debug.WriteLine($"{type} was loaded successfully.");
                            break;
                        }

                        Type foundtype;
                        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
                            foundtype = asm.GetType(type);
                            if (foundtype == null)
                                continue;

                            goto _found;
                        }

                        throw new ExpressionCompileException($"Unable to find type: {type}");
                        _found:
                        Debug.WriteLine($"{type} was loaded successfully.");
                        if (alias != null) {
                            Context.Imports.AddType(foundtype, alias);
                        } else
                            Context.Imports.AddType(foundtype);

                        break;
                    }

                    case ParserToken.Declaration: {
                        var expr = (VariableDeclarationExpression) action.Related.Single();
                        var name = expr.Name.AsString();

                        //validate name
                        {
                            if (InterpreterOptions.BuiltinKeywords.Any(w => w.Equals(name, StringComparison.Ordinal))) {
                                throw new ExpressionCompileException($"Variable named '{name}' is taken by the interpreter.");
                            }
                        }

                        var right = expr.Right;
                        var evaluation = EvaluateExpression(right);
                        Context.Variables[name] = Data.Create(evaluation);
                        break;
                    }

                    case ParserToken.Expression: {
                        var line = action.RelatedLines.Single();
                        if (line.Metadata.Contains("ParserToken.Expression")) {
                            break;
                        }

                        line.Metadata.Add("ParserToken.Expression");
                        line.MarkedForDeletion = false; //they are all true by default, well all lines that were found relevant to ParserAction
                        var copy = line.Content;
                        var ew = new ExpressionWalker(ExpressionLexer.Tokenize(copy));
                        var vars = Context.Variables;
                        bool changed = false;
                        int last_access_index = 0;
                        //we reparse the line and handle all expressions.

                        if (ew.HasNext) {
                            do {
                                _restart:
                                if (changed) {
                                    changed = false;
                                    var cleanedCopy = new string(' ', last_access_index) + copy.Substring(last_access_index);
                                    ew = new ExpressionWalker(ExpressionLexer.Tokenize(cleanedCopy));
                                }

                                var current = ew.Current;
                                //iterate all tokens of that line
                                if (current.Token != ExpressionToken.Mod || !ew.HasNext)
                                    continue;
                                var mod = ew.Current;
                                current = ew.NextToken();
                                switch (current.Token) {
                                    case ExpressionToken.LeftParen: {
                                        //it is an expression.

                                        ew.NextOrThrow();
                                        var expression = Expression.ParseExpression(ew);
                                        object val = EvaluateObject(expression, ew, line);
                                        if (val is ReferenceData rd) //make sure references are unpacked
                                            val = rd.UnpackReference(Context);
                                        ew.IsCurrentOrThrow(ExpressionToken.RightParen);
                                        var emit = val is Data d ? d.Emit() : val.ToString();
                                        copy = copy
                                            .Remove(mod.Match.Index, ew.Current.Match.Index + 1 - mod.Match.Index)
                                            .Insert(mod.Match.Index, emit);
                                        last_access_index = mod.Match.Index + emit.Length;
                                        changed = true;
                                        goto _restart;
                                    }

                                    default:
                                        continue;
                                }
                            } while (ew.Next());
                        }

                        line.Replace(copy + (copy.EndsWith("\n") ? "" : "\n"));
                        break;
                    }


                    case ParserToken.ForeachLoop: {
                        var expr = (ForeachExpression) action.Related.Single();

                        var baseLine = action.RelatedLines.First();
                        var contents = action.RelatedLines.Skip(1).Select(l => l.Content).ToArray();
                        baseLine.MarkedForDeletion = false;
                        var iterateThose = expr.Arguments.Arguments.Select(parseExpr).ToList();
                        unpackPackedArguments();
                        //get smallest index and iterate it.
                        var min = iterateThose.Min(i => i.Count);
                        var vars = Context.Variables;

                        for (int i = 0; i < min; i++) {
                            //set variables
                            vars["i"] = i;
                            for (int j = 0; j < iterateThose.Count; j++) {
                                vars[$"__{j + 1}__"] = iterateThose[j][i];
                            }

                            //now here we iterate contents and set all variables in it.
                            foreach (var content in contents) {
                                //iterate lines, one at a time 
                                var copy = content.ToString();
                                bool changed = false;
                                int last_access_index = 0;
                                const string HashtagExpressionRegex = @"(?<!\\)\#\((?:[^()]|(?<open>\()|(?<-open>\)))+(?(open)(?!))\)";
                                var hashtagExprs = Regex.Matches(copy, HashtagExpressionRegex, Regexes.DefaultRegexOptions).Cast<Match>().ToArray();

                                //replace all emit commands
                                copy = ExpressionLexer.ReplaceRegex(copy, @"(?<!\\)\#([0-9]+)", match => {
                                    //todo here we need to somehow check if the bastard is between #()
                                    var key = $"__{match.Groups[1].Value}__";
                                    if (hashtagExprs.Any(m => m.IsMatchNestedTo(match))) { //it is inside hashtagExpr #(...)
                                        return key;
                                    }
                                    return _emit(vars[key]);
                                });

                                var ew = new ExpressionWalker(ExpressionLexer.Tokenize(copy, ExpressionToken.StringLiteral));

                                if (ew.HasNext) {
                                    do {
                                        _restart:
                                        if (changed) {
                                            changed = false;
                                            var cleanedCopy = new string(' ', last_access_index) + copy.Substring(last_access_index);
                                            ew = new ExpressionWalker(ExpressionLexer.Tokenize(cleanedCopy));
                                        }

                                        var current = ew.Current;
                                        //iterate all tokens of that line
                                        if (current.Token != ExpressionToken.Hashtag || !ew.HasNext)
                                            continue;
                                        var hashtag = ew.Current;
                                        current = ew.NextToken();
                                        switch (current.Token) {
                                            case ExpressionToken.LeftParen: {
                                                //it is an expression.

                                                ew.NextOrThrow();
                                                var expression = Expression.ParseExpression(ew);
                                                object val = EvaluateObject(expression, ew, baseLine);
                                                if (val is ReferenceData rd) //make sure references are unpacked
                                                    val = rd.UnpackReference(Context);
                                                ew.IsCurrentOrThrow(ExpressionToken.RightParen);
                                                var emit = val is Data d ? d.Emit() : val.ToString();
                                                copy = copy
                                                    .Remove(hashtag.Match.Index, ew.Current.Match.Index + 1 - hashtag.Match.Index)
                                                    .Insert(hashtag.Match.Index, emit);
                                                last_access_index = hashtag.Match.Index + emit.Length;
                                                changed = true;
                                                goto _restart;
                                            }

                                            case ExpressionToken.NumberLiteral: {
                                                if (ew.HasNext && ew.PeakNext.Token == ExpressionToken.LeftBracet) {
                                                    //it is an indexer call.
                                                    //todo indexer
                                                } else {
                                                    //it is a simple emit
                                                    var key = $"#{ew.Current.Match.Value}";
                                                    object val = vars[$"__{ew.Current.Match.Value}__"];

                                                    copy = Regex.Replace(copy, Regex.Escape(key), _emit(val));
                                                    changed = true;
                                                }

                                                goto _restart;
                                            }

                                            default:
                                                continue;
                                        }
                                    } while (ew.Next());
                                }

                                baseLine.ReplaceOrAppend(copy + (copy.EndsWith("\n") ? "" : "\n"));
                            }
                        }

                        Context.Variables.Remove("i");
                        for (var i = 0; i < iterateThose.Count; i++) {
                            Context.Variables.Remove($"__{i + 1}__");
                        }


                        IList parseExpr(Expression arg) {
                            var ev = EvaluateObject(arg, null, baseLine);
                            if (ev is ReferenceData d) {
                                ev = d.UnpackReference(Context);
                            }

                            if (ev is StringScalar ss) {
                                return ss.ToCharArray();
                            }

                            if (ev is NetObject no) {
                                ev = no.Value;
                            }

                            return (IList) ev;
                        }

                        void unpackPackedArguments() {
                            //unpack PackedArguments
                            for (var i = iterateThose.Count - 1; i >= 0; i--) {
                                if (iterateThose[i] is PackedArguments pa) {
                                    iterateThose.InsertRange(i, pa.Objects.Select(o => (IList) o));
                                }
                            }

                            iterateThose.RemoveAll(it => it is PackedArguments);
                        }

                        break;
                    }

                    case ParserToken.CopyPaste:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return output.Combine(code.Options);

            string _emit(object val) {
                return val is Data d ? d.Emit() : val.ToString();
            }
        }
    }
}