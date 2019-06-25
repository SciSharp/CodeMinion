using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Regen.Builtins;
using Regen.Compiler.Helpers;
using Regen.DataTypes;
using Regen.DataTypes.Wrappers;
using Regen.Flee.PublicTypes;
using Regen.Helpers;
using Regen.Parser;
using Regen.Parser.Expressions;
using ExpressionCompileException = Regen.Exceptions.ExpressionCompileException;

namespace Regen.Compiler {
    public partial class RegenCompiler : IEvaluator {
        /// <summary>
        ///     Holds all global variables that were created during compiling by <see cref="CompileGlobal"/>.
        /// </summary>
        public Dictionary<string, object> GlobalVariables { get; } = new Dictionary<string, object>();

        public ExpressionContext Context { get; set; }

        public RegenCompiler(params RegenModule[] modules) {
            Context = CreateContext(null, modules);
            Context.Imports.AddInstance(this, "__compiler__");
        }

        public static ExpressionContext CreateContext(Dictionary<string, object> globalVariables = null, RegenModule[] modules = null) {
            // Allow the expression to use all static public methods of System.Math
            var ctx = new ExpressionContext();
            ctx.Options.ParseCulture = CultureInfo.InvariantCulture;
            ctx.Imports.AddType(typeof(Math));
            ctx.Imports.AddType(typeof(CommonExpressionFunctions));
            ctx.Imports.AddType(typeof(CommonLinq));
            ctx.Imports.AddInstance(new CommonRandom(), "random");
            ctx.Imports.AddInstance(ctx, "__context__");
            ctx.Imports.AddInstance(new VariableCollectionWrapper(ctx.Variables), "__vars__");
            ctx.Imports.AddType(typeof(Regex));
            ctx.Variables.ResolveFunction += (sender, args) => { ; };
            ctx.Variables.ResolveVariableType += (sender, args) => { ; };
            ctx.Variables.ResolveVariableValue += (sender, args) => { ; };
            if (modules != null)
                foreach (var mod in modules) {
                    ctx.Imports.AddInstance(mod.Instance, mod.Name);
                }

            if (globalVariables != null)
                foreach (var kv in globalVariables) {
                    ctx.Variables.Add(kv.Key, kv.Value);
                }

            return ctx;
        }

        /// <summary>
        ///     Compiles the given code and stores the variables in current <see cref="Context"/>.
        /// </summary>
        /// <param name="code"></param>
        public void CompileGlobal(string code) {
            //handle global blocks
            //todo compile only variables and expressions (they might affect variables)
        }

        /// <summary>
        ///     Pulls out text from #if _REGEN_GLOBAL blocks
        /// </summary>
        /// <param name="fileText"></param>
        /// <returns></returns>
        public IEnumerable<string> ExtractGlobals(string fileText) {
            //handle global blocks
            string scriptFramed = Regexes.GlobalFrameRegex;
            foreach (Match match in Regex.Matches(fileText, scriptFramed, Regexes.DefaultRegexOptions | RegexOptions.IgnoreCase)) {
                if (!match.Success) //I dont think that unsuccessful can even get here.
                    continue;
                yield return match.Groups[1].Value;
                //after interpretation, they are automatically inserted to the context
            }
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

                                //replace all emit commands
                                copy = ExpressionLexer.ReplaceRegex(copy, @"(?<!\\)\#([0-9]+)", match => { return _emit(vars[$"__{match.Groups[1].Value}__"]); });

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

            return output.Compile(code.Options);

            string _emit(object val) {
                return val is Data d ? d.Emit() : val.ToString();
            }
        }

        #region Modules

        /// <summary>
        ///     Adds or override a module to <see cref="ExpressionContext.Imports"/>.
        /// </summary>
        /// <param name="mod">The module to add</param>
        public void AddModule(RegenModule mod) {
            Context.Imports.AddInstance(mod.Instance, mod.Name);
            //any changes, syncronize with CreateContext.
        }

        /// <summary>
        ///     Adds or override a module to <see cref="ExpressionContext.Imports"/>.
        /// </summary>
        /// <remarks>Used as a simple accessor from a template.</remarks>
        public void AddModule(string name, object instance) {
            Context.Imports.AddInstance(instance, name);
            //any changes, syncronize with CreateContext.
        }

        /// <summary>
        ///     Attempts to remove module from <see cref="ExpressionContext.Imports"/>.
        /// </summary>
        /// <param name="mod">The module to remove</param>
        /// <returns>true if successfully removed</returns>
        public bool RemoveModule(RegenModule mod) {
            var imprt = Context.Imports.RootImport.FirstOrDefault(ib => ib.Name.Equals(mod.Name));
            if (imprt != null) {
                Context.Imports.RootImport.Remove(imprt);
                return !Context.Imports.RootImport.Contains(imprt);
            }

            return false;
        }

        /// <summary>
        ///     Attempts to remove module from <see cref="ExpressionContext.Imports"/>.
        /// </summary>
        /// <param name="moduleName">The module's name to remove</param>
        /// <returns>true if successfully removed</returns>
        public bool RemoveModule(string moduleName) {
            var imprt = Context.Imports.RootImport.FirstOrDefault(ib => ib.Name.Equals(moduleName));
            if (imprt != null) {
                Context.Imports.RootImport.Remove(imprt);
                return !Context.Imports.RootImport.Contains(imprt);
            }

            return false;
        }

        #endregion

        #region Evaluation

        public string EvaluateString(string expression, Line line = null) {
            var evaluated = EvaluateUnpackedObject(expression, line);
            return evaluated as string ?? evaluated?.ToString() ?? "";
        }

        public Int32 EvaluateInt32(string expression, Line line = null) {
            return Evaluate<Int32>(expression, line);
        }

        public T Evaluate<T>(string expression, Line line = null) {
            var evaluated = EvaluateUnpackedObject(expression, line);
            if (typeof(T) == typeof(string))
                return (T) (object) ((evaluated as string) ?? evaluated?.ToString() ?? "");

            return (T) Convert.ChangeType(evaluated, typeof(T));
        }

        public object EvaluateUnpackedObject(string expression, Line line = null) {
            var evaluated = EvaluateObject(expression, line);
            if (evaluated is Data data)
                return data.Value;
            return evaluated;
        }

        public object EvaluateObject(string expression, Line line = null) {
            //core between string to expr methods
            var eew = Expression.GetExpressionWithWalker(expression);
            return EvaluateObject(eew.Expression, eew.ExpressionWalker, line);
        }

        public string EvaluateString(Expression expression, ExpressionWalker ew, Line line = null) {
            var evaluated = EvaluateUnpackedObject(expression, ew, line);
            return evaluated as string ?? evaluated?.ToString() ?? "";
        }

        public Int32 EvaluateInt32(Expression expression, ExpressionWalker ew, Line line = null) {
            return Evaluate<Int32>(expression, ew, line);
        }

        public T Evaluate<T>(Expression expression, ExpressionWalker ew, Line line = null) {
            var evaluated = EvaluateUnpackedObject(expression, ew, line);
            if (typeof(T) == typeof(string))
                return (T) (object) ((evaluated as string) ?? evaluated?.ToString() ?? "");

            return (T) Convert.ChangeType(evaluated, typeof(T));
        }

        public object EvaluateUnpackedObject(Expression expression, ExpressionWalker ew, Line line = null) {
            var evaluated = EvaluateObject(expression, ew, line);
            if (evaluated is Data data)
                return data.Value;
            return evaluated;
        }

        public object EvaluateObject(Expression expression, ExpressionWalker ew, Line line = null) {
            //Core evaluation method.
            try {
                return EvaluateExpression(expression);
            } catch (Flee.PublicTypes.ExpressionCompileException e) {
                throw new Regen.Exceptions.ExpressionCompileException($"Was unable to evaluate expression: {expression}\t  At line ({line?.LineNumber}): {line?.Content}", e);
            }
        }

        #endregion
    }
}