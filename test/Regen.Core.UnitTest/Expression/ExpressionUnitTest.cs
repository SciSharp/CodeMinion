using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Regen.Compiler;
using Regen.Compiler.Digest;
using Regen.Compiler.Expressions;
using Regen.DataTypes;

namespace Regen.Core.Tests.Expression {
    public class ExpressionUnitTest : SharedUnitTestEvaluator
    {
        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Output;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interpreter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public override string Interpret(string code, Dictionary<string, object> variables = null, params RegenModule[] modules) {
            var output = new ExpressionParser(code, code, modules).Interpret(variables);
            Debug(output);
            return output.Output;
        }

        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Variables;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interperter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public override Dictionary<string, object> Variables(string code, Dictionary<string, object> variables = null, params RegenModule[] modules) {
            var output = new ExpressionParser(code, code, modules).Interpret(variables);
            Debug(output);
            return output.Variables;
        }

        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Variables;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interperter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public override InterpredCode Compile(string code, Dictionary<string, object> variables = null, params RegenModule[] modules) {
            var output = new ExpressionParser(code, code, modules).Interpret(variables);
            Debug(output);
            return output;
        }

        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Variables;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interperter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public override Dictionary<string, object> UnpackedVariables(string code, Dictionary<string, object> variables = null, params RegenModule[] modules) {
            var output = new ExpressionParser(code, code, modules).Interpret(variables);
            Debug(output);
            return output?.Variables?.ToDictionary(kv => kv.Key, kv => kv.Value is Data d ? d.Value : kv.Value);
        }

        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Variables;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="allShouldBeOfType">Will assert the following: output.Variables.Values.Should().AllBeOfType(allShouldBeOfType)</param>
        /// <param name="variables">Optional variables to be passed to the interperter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public override Dictionary<string, object> UnpackedVariables(string code, Type allShouldBeOfType, Dictionary<string, object> variables = null, params RegenModule[] modules) {
            var output = new ExpressionParser(code, code, modules).Interpret(variables);
            Debug(output);
            if (allShouldBeOfType != null)
                output.Variables.Values.Should().AllBeOfType(allShouldBeOfType);
            return output?.Variables?.ToDictionary(kv => kv.Key, kv => kv.Value is Data d ? d.Value : kv.Value);
        }
    }
}