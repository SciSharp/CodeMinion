using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Regen.Compiler.Helpers;
using Regen.DataTypes;
using Regen.Exceptions;
using Regen.Helpers;
using Regen.Helpers.Collections;
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

        /// <summary>
        ///     Compiles the given code and stores the variables in current <see cref="Context"/>.
        /// </summary>
        public void CompileGlobal(ParsedCode parsed) {
            //handle global blocks
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
                CompileAction(action, code.ParseActions);
            }

            return output.Combine(code.Options);
        }

        public void CompileAction(ParserAction action, OList<ParserAction> actions) {
            if (action.Consumed)
                return;

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
                                if (ew.Count == 0)
                                    break;
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
                                    object val = EvaluateObject(expression, line);
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
                    _compileForeach(action);
                    break;
                }

                case ParserToken.Template:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private static string _emit(object val) {
            return val is Data d ? d.Emit() : val.ToString();
        }

        private void _compileForeach(ParserAction action) {
            var expr = (ForeachExpression) action.Related.Single();
            var baseline = action.RelatedLines.First();
            baseline.MarkedForDeletion = false;

            _executeForeach(expr, action.RelatedLines.Skip(1).ToList(), baseline);
        }

        private void _executeForeach(ForeachExpression expr, List<Line> relatedLines, Line baseLine) {
            const string HashtagExpressionRegex = @"(?<!\\)\#\((?:[^()]|(?<open>\()|(?<-open>\)))+(?(open)(?!))\)";
            const string HashtagNRegex = @"(?<!\\)\#([0-9]+)";
            var contents = relatedLines.Select(l => l.Content.Replace("|#", "#")).ToArray();
            var readOnlyContent = new List<int>(Enumerable.Repeat(0, contents.Length));

            var iterateThose = expr.Arguments.Arguments.Select(parseExpr).ToList();
            unpackPackedArguments();

            //get smallest index and iterate it.
            var min = iterateThose.Min(i => i.Count);
            var vars = Context.Variables;
            for (int i = 0; i < min; i++) {
                //set variables
                if (expr.Depth > 0)
                    vars[$"i{expr.Depth}"] = new NumberScalar(i);
                else
                    vars["i"] = new NumberScalar(i);
                for (int j = 0; j < iterateThose.Count; j++) {
                    vars[$"__{j + 1 + expr.Depth * 100}__"] = iterateThose[j][i];
                }

                var variables = new List<string>(); //a list of all added variables that will be cleaned after this i iteration.

                //now here we iterate contents and set all variables in it.
                for (var contentIndex = 0; contentIndex < contents.Length; contentIndex++) {
                    var content = contents[contentIndex];
                    var copy = content;

                    if (readOnlyContent[contentIndex] == 1)
                        goto _nextline;

                    //iterate lines, one at a time 
                    // ReSharper disable once RedundantToStringCall
                    bool changed = false;
                    int last_access_index = 0;
                    bool modified = false;
                    //replace all emit commands
                    var hashtagExprs = Regex.Matches(copy, HashtagExpressionRegex, Regexes.DefaultRegexOptions).Cast<Match>().ToArray();
                    copy = ExpressionLexer.ReplaceRegex(copy, HashtagNRegex, match => {
                        var key = $"__{match.Groups[1].Value}__";
                        if (hashtagExprs.Any(m => m.IsMatchNestedTo(match))) {
                            //it is inside hashtagExpr #(...)
                            return key;
                        }

                        modified = true;
                        return _emit(vars[key]);
                    });


                    var ew = new ExpressionWalker(ExpressionLexer.Tokenize(copy, ExpressionToken.StringLiteral));

                    if (ew.HasNext) {
                        do {
                            _restart:
                            if (changed) {
                                changed = false;
                                var cleanedCopy = new string(' ', last_access_index) + copy.Substring(last_access_index);
                                ew = new ExpressionWalker(ExpressionLexer.Tokenize(cleanedCopy, ExpressionToken.StringLiteral));
                            }

                            var current = ew.Current;
                            //iterate all tokens of that line

                            if (current.Token == ExpressionToken.Mod && ew.HasNext) {
                                modified = true;
                                if (ew.HasBack && ew.PeakBack.Token == ExpressionToken.Escape)
                                    continue;

                                var expr_ew = new ExpressionWalker(ExpressionLexer.Tokenize(copy.Substring(current.Match.Index)));
                                //var offset = current.Match.Index;
                                //var hashtag = expr_ew.Current;
                                current = expr_ew.NextToken();
                                switch (current.Token) {
                                    case ExpressionToken.Foreach:
                                        var code = contents.Skip(contentIndex).StringJoin();
                                        var e = ForeachExpression.Parse(code);
                                        var foreachExpr = (ForeachExpression) e.Related[0];
                                        foreachExpr.Depth = expr.Depth + 1;
                                        //no need to mark lines from e for deletion, they are already marked beforehand.
                                        _executeForeach(foreachExpr, e.RelatedLines, baseLine);
                                        contentIndex += e.RelatedLines.Count + 2 - 1; //first for the %foreach line, second for the closer %, -1 because we increment index by one on next iteration.
                                        goto _skipline;
                                    default:
                                        continue;
                                }
                            }

                            if (current.Token == ExpressionToken.Hashtag && ew.HasNext) {
                                modified = true;
                                if (ew.HasBack && ew.PeakBack.Token == ExpressionToken.Escape)
                                    continue;

                                var offset = current.Match.Index;
                                var expr_ew = new ExpressionWalker(ExpressionLexer.Tokenize(copy.Substring(current.Match.Index)));

                                var hashtag = expr_ew.Current;
                                current = expr_ew.NextToken();
                                switch (current.Token) {
                                    case ExpressionToken.Literal:
                                        //this is variable declaration %varname = expr
                                        var peak = expr_ew.PeakNext.Token;
                                        if (peak == ExpressionToken.Equal) {
                                            var e = VariableDeclarationExpression.Parse(expr_ew);
                                            var varname = e.Name.AsString();
                                            if (!Context.Variables.ContainsKey(varname))
                                                variables.Add(varname);
                                            CompileAction(new ParserAction(ParserToken.Declaration, new List<Expression>() {e}), new OList<ParserAction>(0));

                                            goto _skipline;
                                        }

                                        break;
                                    case ExpressionToken.LeftParen: {
                                        //it is an expression.

                                        expr_ew.NextOrThrow();
                                        var expression = Expression.ParseExpression(expr_ew);
                                        object val = EvaluateObject(expression, baseLine);
                                        if (val is ReferenceData rd) //make sure references are unpacked
                                            val = rd.UnpackReference(Context);
                                        expr_ew.IsCurrentOrThrow(ExpressionToken.RightParen);
                                        var emit = val is Data d ? d.Emit() : val.ToString();
                                        copy = copy
                                            .Remove(offset + hashtag.Match.Index, expr_ew.Current.Match.Index + 1 - hashtag.Match.Index)
                                            .Insert(offset + hashtag.Match.Index, emit);
                                        last_access_index = hashtag.Match.Index + emit.Length;
                                        changed = true;
                                        goto _restart;
                                    }

                                    case ExpressionToken.NumberLiteral: {
                                        if (expr_ew.HasNext && expr_ew.PeakNext.Token == ExpressionToken.LeftBracet) {
                                            //it is an indexer call.
                                            //todo indexer
                                        } else {
                                            //it is a simple emit
                                            var key = $"#{expr_ew.Current.Match.Value}";
                                            object val = vars[$"__{expr_ew.Current.Match.Value}__"];

                                            copy = Regex.Replace(copy, Regex.Escape(key), _emit(val));
                                            changed = true;
                                        }

                                        goto _restart;
                                    }

                                    default:
                                        continue;
                                }
                            }

                            //incase it is escaped, continue.
                        } while (ew.Next());
                    }

                    if (!modified)
                        readOnlyContent[contentIndex] = 1;

                    _nextline:
                    //cleanup escapes
                    copy = copy.Replace("\\#", "#");

                    baseLine.ReplaceOrAppend(copy + (copy.EndsWith("\n") ? "" : "\n"));
                    _skipline: ;
                }

                foreach (var variable in variables)
                    Context.Variables.Remove(variable);
            }

            if (expr.Depth == 0)
                Context.Variables.Remove("i");
            else
                Context.Variables.Remove($"i{expr.Depth}");
            for (var i = 0; i < iterateThose.Count; i++) {
                Context.Variables.Remove($"__{i + 1 + expr.Depth * 100}__");
            }

            if (!baseLine.ContentWasModified)
                baseLine.MarkedForDeletion = true;

            IList parseExpr(Expression arg) {
                var ev = EvaluateObject(arg, baseLine);
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
        }
    }
}