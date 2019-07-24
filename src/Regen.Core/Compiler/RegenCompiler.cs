using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
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

        private static readonly ThreadLocal<ExpressionContext> _threadLocalContext = new ThreadLocal<ExpressionContext>();

        private static readonly ThreadLocal<RegenCompiler> _threadLocalCompiler = new ThreadLocal<RegenCompiler>();

        /// <summary>
        ///     Gets the current context that is executing the expression.
        /// </summary>
        /// <remarks>Can be null if not used properly.</remarks>
        public static ExpressionContext CurrentContext => _threadLocalContext.Value;

        /// <summary>
        ///     Gets the current compiler that is executing the expression.
        /// </summary>
        /// <remarks>Can be null if not used properly.</remarks>
        public static RegenCompiler CurrentCompiler => _threadLocalCompiler.Value;

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
            return EvaluateObject(eew.Expression,  line);
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
            var evaluated = EvaluateObject(expression, line);
            if (evaluated is Data data)
                return data.Value;
            return evaluated;
        }

        public object EvaluateObject(Expression expression, Line line = null) {
            //Core evaluation method.
            _threadLocalContext.Value = Context;
            _threadLocalCompiler.Value = this;
            try {
                return EvaluateExpression(expression);
            } catch (Flee.PublicTypes.ExpressionCompileException e) {
                throw new Regen.Exceptions.ExpressionCompileException($"Was unable to evaluate expression: {expression}\t  At line ({line?.LineNumber}): {line?.Content}", e);
            } finally {
                _threadLocalContext.Value = null;
                _threadLocalCompiler.Value = null;
            }
        }

        #endregion
    }
}