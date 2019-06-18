using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Regen.Compiler;
using Regen.DataTypes;

namespace Regen.Core.Tests {
    public abstract class UnitTestBase {
        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Output;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interperter</param>
        public string Interpert(string code, Dictionary<string, Data> variables = null) {
            var output = new Interperter(code, code).Run(variables);
            Debug(output);
            return output.Output;
        }

        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Variables;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interperter</param>
        public Dictionary<string, Data> Variables(string code, Dictionary<string, Data> variables = null) {
            var output = new Interperter(code, code).Run(variables);
            Debug(output);
            return output.Variables;
        }
        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Variables;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interperter</param>
        public Dictionary<string, object> UnpackedVariables(string code, Dictionary<string, Data> variables = null) {
            var output = new Interperter(code, code).Run(variables);
            Debug(output);
            return output?.Variables?.ToDictionary(kv=>kv.Key, kv=>kv.Value.Value);
        }

        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Variables;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="allShouldBeOfType">Will assert the following: output.Variables.Values.Should().AllBeOfType(allShouldBeOfType)</param>
        /// <param name="variables">Optional variables to be passed to the interperter</param>
        public Dictionary<string, object> UnpackedVariables(string code, Type allShouldBeOfType, Dictionary<string, Data> variables = null) {
            var output = new Interperter(code, code).Run(variables);
            Debug(output);
            if (allShouldBeOfType != null)
                output.Variables.Values.Should().AllBeOfType(allShouldBeOfType);
            return output?.Variables?.ToDictionary(kv=>kv.Key, kv=>kv.Value.Value);
        }

        public static void Debug(InterpredCode output) {
            Console.WriteLine("Output:  -----------------");
            Console.WriteLine(output.Output);
            Console.WriteLine("Variables: ---------------");
            foreach (var kv in output.Variables) {
                Console.WriteLine($"{kv.Key}:\t\t{kv.Value}");
            }
        }
    }
}