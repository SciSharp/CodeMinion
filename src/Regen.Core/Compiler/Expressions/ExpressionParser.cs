using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Flee.PublicTypes;
using Regen.Builtins;
using Regen.Collections;
using Regen.DataTypes;
using Regen.Exceptions;
using Regen.Helpers;
using Regen.Wrappers;
using Array = Regen.DataTypes.Array;
using ExpressionCompileException = Regen.Exceptions.ExpressionCompileException;

namespace Regen.Compiler {
    /// <summary>
    ///     A parser finds #if <see cref="DefineMarker"/>, passes contents to an <see cref="Interpreter"/> and then places the output inside the #else block.
    /// </summary>
    /// <remarks>If you are looking to just parse a template without regions and so on, please refer to <see cref="Interpreter"/>.<see cref="Interpreter.Interpret(System.Collections.Generic.Dictionary{string,Regen.DataTypes.Data})"/></remarks>
    public class ExpressionParser : IEvaluator {
        public ExpressionParser(string entireCode, string regenCode, params RegenModule[] modules) {
            EntireCode = entireCode + Environment.NewLine;
            RegenCode = regenCode + Environment.NewLine;
            ReversedRegenCode = new string(regenCode.Reverse().ToArray());

            Context = CreateContext(null, modules);
            Context.Imports.AddInstance(this, "__interpreter__");

            //handle global blocks
            GlobalVariables = new Dictionary<string, object>();
            string scriptFramed = Parser.GlobalFrameRegex;
            foreach (Match match in Regex.Matches(entireCode, scriptFramed, Regexes.DefaultRegexOptions)) {
                if (!match.Success) //I dont think that unsuccessful can even get here.
                    continue;

                //after interpretation, they are automatically inserted to the context
                GlobalVariables.AddRange(InterpretGlobal(match.Groups[1].Value));
            }
        }

        /// <summary>
        ///     Holds all global variables that were generated during the global blocks parse.
        /// </summary>
        public Dictionary<string, object> GlobalVariables { get; }

        public InterpreterOptions Options { get; set; } = new InterpreterOptions();

        public string EntireCode { get; }
        public string RegenCode { get; set; }
        private string ReversedRegenCode { get; set; }
        public ExpressionContext Context { get; set; }

        public static ExpressionContext CreateContext(Dictionary<string, object> globalVariables = null, RegenModule[] modules = null) {
            // Allow the expression to use all static public methods of System.Math
            var ctx = new ExpressionContext();
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
            return Evaluate<string>(expression, line);
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
            if (evaluated is Data scalar)
                return scalar.Value;
            return evaluated;
        }

        public object EvaluateObject(string expression, Line line = null) {
            bool reattempted = false;
            _attemptReevaluate:
            try {
                return Context.CompileDynamic(expression).Evaluate();
            } catch (Flee.PublicTypes.ExpressionCompileException e) when (e.Message.Contains("ArithmeticElement") && e.Message.Contains("Null") && e.Message.Contains("Operation")) {
                if (reattempted)
                    throw;
                expression = expression.Replace("null", "0");
                reattempted = true;
                goto _attemptReevaluate;
            } catch (Flee.PublicTypes.ExpressionCompileException e) {
                throw new ExpressionCompileException($"Was unable to evaluate expression: {expression}\t  At line ({line?.LineNumber}): {line?.Content}", e);
            }
        }

        #endregion

        public Dictionary<string, object> InterpretGlobal(string code, Dictionary<string, object> variables = null) {
            return Interpret(code, variables).Variables;
        }

        public InterpredCode Interpret(Dictionary<string, object> variables = null) {
            return Interpret(RegenCode, variables);
        }

        public InterpredCode Interpret(string code, Dictionary<string, object> variables = null) {
            const string unescapeCommentRegex = @"(\\\#\/\/)";
            //clean code from comments
            code = Regex.Replace(code, ExpressionToken.CommentRow.GetAttribute<DescriptionAttribute>().Description, new MatchEvaluator(match => { return match.Value.Replace(match.Groups[1].Value, ""); }), Regexes.DefaultRegexOptions);
            code = Regex.Replace(code, unescapeCommentRegex, @"//", Regexes.DefaultRegexOptions); //unescape escaped comments

            var lines = new LineBuilder(code);
            var output = lines.Clone();

            if (variables != null) {
                foreach (var variable in variables) {
                    Context.Variables.Add(variable.Key, variable.Value);
                }
            } else
                variables = new Dictionary<string, object>();

            // Define the context of our expression
            var walker = ExpressionLexer.Tokenize(code).WrapWalker();
            if (walker.Count == 0) {
                //no tokens detected
                var comp = output.Compile(Options);
                return new InterpredCode() {OriginalCode = code, Output = comp, Variables = Context.Variables.ToDictionary(kv => kv.Key, kv => kv.Value)};
            }

            do {
                var current = walker.Current;
                ;
                switch (walker.Current.ExpressionToken) {
                    case ExpressionToken.Import:
                        break;
                    case ExpressionToken.Function:
                        break;
                    case ExpressionToken.If:
                        break;
                    case ExpressionToken.ElseIf:
                        break;
                    case ExpressionToken.Else:
                        break;
                    case ExpressionToken.Foreach:
                        break;
                    case ExpressionToken.Return:
                        break;
                    case ExpressionToken.StringLiteral:
                        break;
                    case ExpressionToken.NumberLiteral:
                        break;
                    case ExpressionToken.Identity:
                        break;
                    case ExpressionToken.Whitespace:
                        break;
                    case ExpressionToken.NewLine:
                        break;
                    case ExpressionToken.Add:
                        break;
                    case ExpressionToken.Sub:
                        break;
                    case ExpressionToken.Mul:
                        break;
                    case ExpressionToken.Div:
                        break;
                    case ExpressionToken.DoubleEqual:
                        break;
                    case ExpressionToken.NotEqual:
                        break;
                    case ExpressionToken.Equal:
                        break;
                    case ExpressionToken.LeftBrace:
                        break;
                    case ExpressionToken.RightBrace:
                        break;
                    case ExpressionToken.LeftBrackets:
                        break;
                    case ExpressionToken.RightBrackets:
                        break;
                    case ExpressionToken.Comma:
                        break;
                    case ExpressionToken.Period:
                        break;
                    case ExpressionToken.Lambda:
                        break;
                    case ExpressionToken.CommentRow:
                        break;
                    case ExpressionToken.Block:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            } while (walker.Next());

            var compiled = output.Compile(Options);
            return new InterpredCode() {OriginalCode = code, Output = compiled, Variables = variables};
        }

        public string ExpandVariables(Line line, int stackIndex, Dictionary<int, StackDictionary> stacks) {
            return null;
            //var code = line.Content;
            //var basicEmits = DigestLexer.FindTokens(ExpressionToken.EmitVariable, code);
            //var currentStack = stacks[stackIndex];

            ////get tokens with expression in them.
            //var offsetEmit = DigestLexer.FindTokens(ExpressionToken.EmitVariableOffsetted, code);
            //var expressionEmits = DigestLexer.FindTokens(ExpressionToken.EmitExpression, code);
            //int additionalIndex = 0;
            //foreach (var emits in basicEmits.GroupBy(e => Convert.ToInt32(e.Match.Groups[1].Value))) {
            //    var index = emits.Key;

            //    foreach (var emit in emits.OrderBy(e => e.Match.Index)) {
            //        if (!currentStack.ContainsKey(index))
            //            throw new IndexOutOfRangeException($"Index #{index} at line {line.LineNumber} not found during emit at block: {code}");
            //        var isnested = offsetEmit.Any(m => m.Match.IsMatchNestedTo(emit.Match)) || expressionEmits.Any(m => m.Match.IsMatchNestedTo(emit.Match));
            //        code = code.Remove(emit.Match.Index + additionalIndex, emit.Match.Length);
            //        var emit_text = isnested ? currentStack[index].EmitExpressive() : currentStack[index].Emit();
            //        code = code.Insert(emit.Match.Index + additionalIndex, emit_text);
            //        additionalIndex += emit_text.Length - emit.Match.Length;
            //    }
            //}

            //offsetEmit = DigestLexer.FindTokens(ExpressionToken.EmitVariableOffsetted, code); //re-lex because of expressions that might have been expanded inside.
            //additionalIndex = 0; //because we re-lexxed
            //foreach (var emits in offsetEmit.GroupBy(e => Convert.ToInt32(e.Match.Groups[1].Value))) {
            //    var index = emits.Key;

            //    foreach (var emit in emits.OrderBy(e => e.Match.Index)) {
            //        if (!currentStack.ContainsKey(index))
            //            throw new IndexOutOfRangeException($"Index #{index} at line {line.LineNumber} not found during emit at block: {code}");

            //        var expression = emit.Match.Groups[2].Value;
            //        var accessorStackIndex = EvaluateInt32(expression, line) + 1; //during expressions for loop index is 0 based, but stack is 1.
            //        string emit_text;
            //        try {
            //            emit_text = stacks[accessorStackIndex][index].Emit();
            //        } catch (KeyNotFoundException) {
            //            code = code.Remove(emit.Match.Index + additionalIndex, emit.Match.Length);
            //            additionalIndex -= emit.Match.Length;

            //            continue; //no emits now.
            //        }

            //        code = code.Remove(emit.Match.Index + additionalIndex, emit.Match.Length);
            //        code = code.Insert(emit.Match.Index + additionalIndex, emit_text);
            //        additionalIndex += emit_text.Length - emit.Match.Length;
            //    }
            //}

            ////by now everything should be expanded, so we just evaluate and replace.
            //expressionEmits = DigestLexer.FindTokens(ExpressionToken.EmitExpression, code);
            //additionalIndex = 0;
            //foreach (var expressionMatch in expressionEmits) {
            //    var expression = expressionMatch.Match.Groups[1].Value;
            //    string emit = EvaluateString(expression, line);

            //    code = code.Remove(expressionMatch.Match.Index + additionalIndex, expressionMatch.Match.Length);
            //    code = code.Insert(expressionMatch.Match.Index + additionalIndex, emit);
            //    additionalIndex += emit.Length - expressionMatch.Match.Length;
            //}

            //return code;
        }

        public class ForLoop {
            public int From { get; set; }
            public int To { get; set; }
            public int Index { get; set; }

            public bool CanNext() => Index < To;

            public int Next() {
                if (CanNext()) {
                    var currently = Index;
                    Index++;
                    return currently;
                }

                return 0;
            }
        }
    }
}