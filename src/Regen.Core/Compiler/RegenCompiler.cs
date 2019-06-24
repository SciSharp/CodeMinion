using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Flee.PublicTypes;
using Microsoft.CSharp.RuntimeBinder;
using Regen.Builtins;
using Regen.Compiler.Digest;
using Regen.Compiler.Helpers;
using Regen.DataTypes;
using Regen.Helpers;
using Regen.Wrappers;
using Array = Regen.DataTypes.Array;
using ExpressionCompileException = Regen.Exceptions.ExpressionCompileException;

namespace Regen.Compiler.Expressions {
    public partial class RegenCompiler : IEvaluator {
        /// <summary>
        ///     Holds all global variables that were created during compiling by <see cref="CompileGlobal"/>.
        /// </summary>
        public Dictionary<string, object> GlobalVariables { get; } = new Dictionary<string, object>();

        public ExpressionContext Context { get; set; }

        public RegenCompiler(params RegenModule[] modules) {
            Context = CreateContext(null, modules);
            Context.Imports.AddInstance(this, "__interpreter__");
        }

        public static ExpressionContext CreateContext(Dictionary<string, object> globalVariables = null, RegenModule[] modules = null) {
            // Allow the expression to use all static public methods of System.Math
            var ctx = new ExpressionContext();
            ctx.Options.ParseCulture = CultureInfo.InvariantCulture;
            ctx.Imports.AddType(typeof(Math));
            ctx.Imports.AddType(typeof(CommonExpressionFunctions));
            ctx.Imports.AddInstance(new CommonRandom(), "random");
            ctx.Imports.AddInstance(ctx, "__context__");
            ctx.Imports.AddInstance(new VariableCollectionWrapper(ctx.Variables), "__vars__");
            ctx.Imports.AddType(typeof(Regex));

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
            var output = code.Output;
            var variables = code.Variables ?? new Dictionary<string, object>();

            foreach (var globalVariable in GlobalVariables)
                variables.Add(globalVariable.Key, globalVariable.Value);

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
                        var expr = (Expression) action.Related.Single();
                        var line = action.RelatedLines.Single();
                        var evaluated = EvaluateString(expr.AsString(), line);

                        line.MarkedForDeletion = false; //they are all true by default, well all lines that were found relevant to ParserAction
                        line.Replace(line.Prepends + evaluated ?? "");
                        break;
                    }

                    case ParserToken.ForeachLoop: {
                        var expr = (ForeachExpression) action.Related.Single();

                        var baseLine = action.RelatedLines.First();
                        var contents = action.RelatedLines.Skip(1).Select(l => (Line: l.Content, ExpressionLexer.Tokenize(l.Content))).ToArray();
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

                            //todo now here we iterate contents and set all variables in it.
                            foreach (var (line, tkns) in contents) {
                                //iterate lines, one at a time 
                                var copy = line.ToString();
                                bool changed = false;
                                var ew = new ExpressionWalker(tkns);

                                //var byregex = ExpressionLexer.ReplaceRegex(copy, @"(?<!\\)\#([0-9]+)", "__$1__");

                                if (ew.HasNext) {
                                    do {
                                        _restart:
                                        if (changed) {
                                            changed = false;
                                            ew = new ExpressionWalker(ExpressionLexer.Tokenize(copy));
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

                                                ew.IsCurrentOrThrow(ExpressionToken.RightParen);
                                                copy = copy
                                                    .Remove(hashtag.Match.Index, ew.Current.Match.Index + 1 - hashtag.Match.Index)
                                                    .Insert(hashtag.Match.Index, val is Data d ? d.Emit() : val.ToString());
                                                changed = true;
                                                goto _restart;
                                            }

                                            case ExpressionToken.NumberLiteral: {
                                                if (ew.HasNext && ew.PeakNext.Token == ExpressionToken.LeftBracet) {
                                                    //it is an indexer call.
                                                } else {
                                                    //it is a simple emit
                                                    var key = $"#{ew.Current.Match.Value}";
                                                    object val = vars[$"__{ew.Current.Match.Value}__"];

                                                    copy = Regex.Replace(copy, Regex.Escape(key), val is Data d ? d.Emit() : val.ToString());
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
                return EvaluateExpression(expression); //todo here we need to assess wth is going on by ourselves first. like expandvariable  
            } catch (Flee.PublicTypes.ExpressionCompileException e) {
                throw new Regen.Exceptions.ExpressionCompileException($"Was unable to evaluate expression: {expression}\t  At line ({line?.LineNumber}): {line?.Content}", e);
            }
        }
        
        #endregion
    }
}