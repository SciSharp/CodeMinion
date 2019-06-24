using System;
using System.Collections.Generic;
using System.Linq;
using Flee.PublicTypes;
using FluentAssertions;
using Regen.Compiler;
using Regen.Compiler.Digest;
using Regen.DataTypes;

namespace Regen.Core.Tests.Digest {
    public abstract class DigestUnitTestEvaluator : SharedUnitTestEvaluator {
        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Output;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interpreter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public override ParsedCode Parse(string code, Dictionary<string, object> variables = null, params RegenModule[] modules) {
            var output = new Interpreter(code, code, modules).Interpret(variables);
            Debug(output);
            return output;
        }

        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Variables;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interperter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public override VariableCollection Variables(string code, Dictionary<string, object> variables = null, params RegenModule[] modules) {
            var output = new Interpreter(code, code, modules).Interpret(variables);
            Debug(output);
            return null;
        }

        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Variables;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interperter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public override (ParsedCode ParsedCode, string Output) Compile(string code, Dictionary<string, object> variables = null, params RegenModule[] modules) {
            var output = new Interpreter(code, code, modules).Interpret(variables);
            Debug(output);
            return (output, output.Output.Compile(output.Options));
        }

        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Variables;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interperter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public override Dictionary<string, object> UnpackedVariables(string code, Dictionary<string, object> variables = null, params RegenModule[] modules) {
            var output = new Interpreter(code, code, modules).Interpret(variables);
            Debug(output);
            return output?.Variables?.ToDictionary(kv => kv.Key, kv => kv.Value is Data d ? d.Value : kv.Value);
        }
    }
}