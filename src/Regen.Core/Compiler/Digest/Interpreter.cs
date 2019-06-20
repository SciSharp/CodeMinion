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
using Regen.Compiler.Helpers;
using Regen.DataTypes;
using Regen.Exceptions;
using Regen.Helpers;
using Regen.Wrappers;
using Array = Regen.DataTypes.Array;
using ExpressionCompileException = Regen.Exceptions.ExpressionCompileException;

namespace Regen.Compiler.Digest {
    /// <summary>
    ///     Represents a module that is accessible inside an expression. see remarks.
    /// </summary>
    /// <remarks>Example usage: %(<see cref="Name"/>.<see cref="Instance"/>Method(123))</remarks>
    public class RegenModule {
        /// <summary>
        ///     The module name that will later can be accessed %(name.method(123))
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The module object that will be used to import functions into <see cref="Flee"/>.
        /// </summary>
        /// <remarks>All public methods will be imported, including static ones.<br></br>Void methods by default return object null.</remarks>
        public object Instance { get; set; }

        public RegenModule(string name, object instance) {
            Name = name;
            Instance = instance;
        }
    }

    /// <summary>
    ///     The interpreter parses, compiles and then converts given code and returns it as a string.
    /// </summary>
    public class Interpreter : IEvaluator {
        public Interpreter(string entireCode, string regenCode, params RegenModule[] modules) {
            EntireCode = entireCode + Environment.NewLine;
            RegenCode = regenCode + Environment.NewLine;
            ReversedRegenCode = new string(regenCode.Reverse().ToArray());

            Context = CreateContext(null, modules);
            Context.Imports.AddInstance(this, "__interpreter__");

            //handle global blocks
            GlobalVariables = new Dictionary<string, object>();
            string scriptFramed = DigestParser.GlobalFrameRegex;
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
            code = Regex.Replace(code, DigestToken.CommentRow.GetAttribute<DescriptionAttribute>().Description, new MatchEvaluator(match => { return match.Value.Replace(match.Groups[1].Value, ""); }), Regexes.DefaultRegexOptions);
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
            var walker = DigestLexer.Tokenize(code).WrapWalker();
            if (walker.Count == 0) {
                //no tokens detected
                var comp = output.Compile(Options);
                return new InterpredCode() {OriginalCode = code, Output = comp, Variables = Context.Variables.ToDictionary(kv => kv.Key, kv => kv.Value)};
            }

            do {
                var current = walker.Current;
                switch (walker.Current.Token) {
                    case DigestToken.Declaration: {
                        var name = walker.Current.Match.Groups[1].Value.Trim();
                        var line = output.GetLineAt(walker.Current.Match.Index);
                        line.MarkedForDeletion = true; //just because the line has declaration - regardless to whats inside.
                        if (!walker.Next())
                            throw new UnexpectedTokenException<DigestToken>(current, DigestToken.Array);

                        //check if name is valid (C# compliant)
                        if (!name.All(c => char.IsDigit(c) || char.IsLetter(c) || Regexes.VariableNameValidSymbols.Any(cc => cc == c)) || name.TakeWhile(char.IsDigit).Any()) {
                            throw new ExpressionCompileException($"Variable named '{name}' contains invalid characters. Name can only start with a letter or underscore and contain letters, numbers or underscores");
                        }

                        //check interpreter buildin keywords
                        var matchedTakenName = InterpreterOptions.BuiltinKeywords.FirstOrDefault(w => w.Equals(name, StringComparison.Ordinal));
                        if (matchedTakenName != null) {
                            throw new ExpressionCompileException($"Variable named '{name}' is taken by the interpreter.");
                        }

                        if (walker.Current.Token == DigestToken.Array) {
                            var arrayToken = walker.Current;
                            var arrayStr = arrayToken.Match.Groups[0].Value;

                            var values = Array.Parse(arrayStr, Context.Variables, this);
                            if (variables.ContainsKey(name))
                                Debug.WriteLine($"Warning: variable named {name} is already taken and is being overridden at TODO PRINT LINE");
                            variables[name] = values;
                            Context.Variables[name] = values;
                        } else if (walker.Current.Token == DigestToken.Scalar) {
                            var scalarToken = walker.Current;
                            var scalarStr = scalarToken.Match.Groups[1].Value.TrimEnd('\n', '\r');

                            object value;
                            if (int.TryParse(scalarStr, out var @long)) {
                                value = @long;
                            } else if (decimal.TryParse(scalarStr, out var @decimal)) {
                                value = @decimal;
                            } else if (float.TryParse(scalarStr, out var @float)) {
                                value = @float;
                            } else {
                                value = EvaluateUnpackedObject(scalarStr);
                            }

                            if (variables.ContainsKey(name))
                                Debug.WriteLine($"Warning: variable named {name} is already taken and is being overridden at TODO PRINT LINE");
                            value = Data.Create(value);
                            variables[name] = (Data) value;
                            Context.Variables[name] = value;
                        }

                        break;
                    }

                    case DigestToken.Expression: {
                        var line = output.GetLineAt(walker.Current.Match.Index);
                        GroupCollection groups;
                        bool originalToken = true;
                        if (line.ContentWasModified) {
                            //re-express
                            var tokens = DigestLexer.FindTokens(DigestToken.Expression, line.Content);
                            if (tokens.Count == 0)
                                break;
                            groups = tokens[0].Match.Groups;
                            originalToken = false;
                        } else
                            groups = walker.Current.Match.Groups;

                        var expression = groups[1].Value;


                        var evaluation = EvaluateString(expression);
                        var lineStr = line.Content
                            .Remove(groups[0].Index - (originalToken ? line.StartIndex : 0), groups[0].Length)
                            .Insert(groups[0].Index - (originalToken ? line.StartIndex : 0), evaluation);
                        line.Replace(lineStr);
                        break;
                    }

                    case DigestToken.ForEach: {
                        var line = lines.GetLineAt(walker.Current.Match.Index);
                        output.FindLine(line).MarkedForDeletion = true;
                        var content = line.Content.Replace("%foreach", "").Replace("%for", "").TrimStart('\t', ' ').TrimEnd('\n', '\r', ' ');
                        var usesBlock = content.EndsWith("%");
                        content = content.TrimEnd('%');

                        //find all relevant variables
                        var parsedVaraibles = new List<object>();
                        foreach (var name in content.Split('|').Select(n => n.Trim(' ', '\r', '\n').TrimEnd('%'))) {
                            //todo ignore escaped \|,
                            if (variables.ContainsKey(name)) {
                                var var = variables[name];
                                if (var is Array) {
                                    parsedVaraibles.Add(var);
                                    continue;
                                }
                            }

                            var obj = EvaluateUnpackedObject(name, line);
                            if (obj is PackedArguments args) {
                                parsedVaraibles.AddRange(args.Objects);
                            } else
                                parsedVaraibles.Add(obj);
                        }

                        var feInstance = new ForeachInstance(parsedVaraibles, ForeachInstance.StackLength.SmallestIndex);
                        var loop = feInstance.StartLoop();

                        if (!usesBlock) {
                            if (line.LineNumber >= lines.Lines.Count)
                                throw new UnexpectedTokenException<DigestToken>("After non-block foreach, theres suppose to be a line that is replicated.");

                            var nextline = lines.Lines[line.LineNumber]; //linenumber is index+1 so we dont need to +1.
                            while (loop.CanNext()) {
                                var currentIndex = loop.Next();
                                Context.Variables["i"] = currentIndex;
                                var expand = ExpandVariables(nextline, currentIndex, feInstance.Stacks);
                                var expandedLine = output.Lines[line.LineNumber]; //no need to do +1, line number is 1 based.

                                expandedLine.ReplaceOrAppend(expand);
                            }

                            Context.Variables.Remove("i");
                        } else {
                            var block = lines.Lines.Skip(line.LineNumber).TakeWhileIncluding(l => l.CleanContent() != "%").ToArray();
                            output.FindLine(block.Last()).MarkedForDeletion = true; //the last marker that will be marked for deletion.

                            block = block.SkipLast(1).ToArray();

                            if (block.Length > 1)
                                foreach (var todeleteLine in block.Skip(1)) {
                                    output.Lines.Single(l => l == todeleteLine).MarkedForDeletion = true;
                                }

                            var appender = output.Lines.Single(l => l == block[0]); //the line we append to

                            while (loop.CanNext()) {
                                var currentIndex = loop.Next();
                                Context.Variables["i"] = currentIndex;
                                foreach (var lineInBlock in block) {
                                    var expanded = ExpandVariables(lineInBlock, currentIndex, feInstance.Stacks);
                                    appender.ReplaceOrAppend(expanded);
                                }
                            }

                            Context.Variables.Remove("i");
                        }

                        break;
                    }

                    case DigestToken.Import: {
                        //todo add support for %import ns.type as name
                        var type = walker.Current.Match.Groups[1].Value.Trim();
                        var line = output.GetLineAt(walker.Current.Match.Index);
                        line.MarkedForDeletion = true; //just because the line has declaration - regardless to whats inside.

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
                        if (walker.Current.Match.Groups.Count >= 3) {
                            Context.Imports.AddType(foundtype, walker.Current.Match.Groups[2].Value);
                        } else
                            Context.Imports.AddType(foundtype);

                        break;
                    }

                    case DigestToken.Scalar:
                    case DigestToken.Array:
                    case DigestToken.EmitVariable:
                    case DigestToken.EmitVariableOffsetted:
                    case DigestToken.BlockMark:
                        //there are tokens that are handles internally by the tokens implemented above.
                        //if they are not inside, they are to be skipped
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            } while (walker.Next());

            var compiled = output.Compile(Options);
            return new InterpredCode() {OriginalCode = code, Output = compiled, Variables = variables};
        }

        public string ExpandVariables(Line line, int stackIndex, Dictionary<int, StackDictionary> stacks) {
            var code = line.Content;
            var basicEmits = DigestLexer.FindTokens(DigestToken.EmitVariable, code);
            var currentStack = stacks[stackIndex];

            //get tokens with expression in them.
            var offsetEmit = DigestLexer.FindTokens(DigestToken.EmitVariableOffsetted, code);
            var expressionEmits = DigestLexer.FindTokens(DigestToken.EmitExpression, code);
            int additionalIndex = 0;
            foreach (var emits in basicEmits.GroupBy(e => Convert.ToInt32(e.Match.Groups[1].Value))) {
                var index = emits.Key;

                foreach (var emit in emits.OrderBy(e => e.Match.Index)) {
                    if (!currentStack.ContainsKey(index))
                        throw new IndexOutOfRangeException($"Index #{index} at line {line.LineNumber} not found during emit at block: {code}");
                    var isnested = offsetEmit.Any(m => m.Match.IsMatchNestedTo(emit.Match)) || expressionEmits.Any(m => m.Match.IsMatchNestedTo(emit.Match));
                    code = code.Remove(emit.Match.Index + additionalIndex, emit.Match.Length);
                    var emit_text = isnested ? currentStack[index].EmitExpressive() : currentStack[index].Emit();
                    code = code.Insert(emit.Match.Index + additionalIndex, emit_text);
                    additionalIndex += emit_text.Length - emit.Match.Length;
                }
            }

            offsetEmit = DigestLexer.FindTokens(DigestToken.EmitVariableOffsetted, code); //re-lex because of expressions that might have been expanded inside.
            additionalIndex = 0; //because we re-lexxed
            foreach (var emits in offsetEmit.GroupBy(e => Convert.ToInt32(e.Match.Groups[1].Value))) {
                var index = emits.Key;

                foreach (var emit in emits.OrderBy(e => e.Match.Index)) {
                    if (!currentStack.ContainsKey(index))
                        throw new IndexOutOfRangeException($"Index #{index} at line {line.LineNumber} not found during emit at block: {code}");

                    var expression = emit.Match.Groups[2].Value;
                    var accessorStackIndex = EvaluateInt32(expression, line) + 1; //during expressions for loop index is 0 based, but stack is 1.
                    string emit_text;
                    try {
                        emit_text = stacks[accessorStackIndex][index].Emit();
                    } catch (KeyNotFoundException) {
                        code = code.Remove(emit.Match.Index + additionalIndex, emit.Match.Length);
                        additionalIndex -= emit.Match.Length;

                        continue; //no emits now.
                    }

                    code = code.Remove(emit.Match.Index + additionalIndex, emit.Match.Length);
                    code = code.Insert(emit.Match.Index + additionalIndex, emit_text);
                    additionalIndex += emit_text.Length - emit.Match.Length;
                }
            }

            //by now everything should be expanded, so we just evaluate and replace.
            expressionEmits = DigestLexer.FindTokens(DigestToken.EmitExpression, code);
            additionalIndex = 0;
            foreach (var expressionMatch in expressionEmits) {
                var expression = expressionMatch.Match.Groups[1].Value;
                string emit = EvaluateString(expression, line);

                code = code.Remove(expressionMatch.Match.Index + additionalIndex, expressionMatch.Match.Length);
                code = code.Insert(expressionMatch.Match.Index + additionalIndex, emit);
                additionalIndex += emit.Length - expressionMatch.Match.Length;
            }

            return code;
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