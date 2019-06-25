using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Regen.Compiler;
using Regen.Compiler.Expressions;
using Regen.DataTypes;
using Regen.Flee.PublicTypes;
using Regen.Parser;

namespace Regen.Core.Tests.Expression {
    public class ExpressionUnitTest : SharedUnitTestEvaluator {
        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Output;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interpreter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public override ParsedCode Parse(string code, Dictionary<string, object> variables = null, params RegenModule[] modules) {
            var parsed = ExpressionParser.Parse(code, variables);
            Debug(parsed);
            return parsed;
        }

        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Variables;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interperter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public override VariableCollection Variables(string code, Dictionary<string, object> variables = null, params RegenModule[] modules) {
            var parsed = ExpressionParser.Parse(code, variables);
            var comp = new RegenCompiler(modules);
            var output = comp.Compile(parsed);

            Debug(output, comp.Context.Variables);
            return comp.Context.Variables;
        }

        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Variables;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interperter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public override (ParsedCode ParsedCode, string Output) Compile(string code, Dictionary<string, object> variables = null, params RegenModule[] modules) {
            var parsed = ExpressionParser.Parse(code, variables);
            var comp = new RegenCompiler(modules);
            var output = comp.Compile(parsed);

            Debug(output, comp.Context.Variables);
            return (parsed, output);
        }

        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Variables;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interperter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public override Dictionary<string, object> UnpackedVariables(string code, Dictionary<string, object> variables = null, params RegenModule[] modules) {
            var parsed = ExpressionParser.Parse(code, variables);
            var comp = new RegenCompiler(modules);
            var output = comp.Compile(parsed);

            Debug(output, comp.Context.Variables);
            return comp.Context.Variables.ToDictionary(kv => kv.Key, kv => kv.Value is Data d ? d.Value : kv.Value);
        }
    }
}