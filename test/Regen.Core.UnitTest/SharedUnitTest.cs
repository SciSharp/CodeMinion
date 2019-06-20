using System;
using System.Collections.Generic;
using Regen.Compiler;
using Regen.DataTypes;

namespace Regen.Core.Tests {
    public abstract class SharedUnitTest {
        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Output;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interpreter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public abstract string Interpret(string code, Dictionary<string, object> variables = null, params RegenModule[] modules);

        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Variables;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interperter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public abstract Dictionary<string, object> Variables(string code, Dictionary<string, object> variables = null, params RegenModule[] modules);

        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Variables;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interperter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public abstract InterpredCode Compile(string code, Dictionary<string, object> variables = null, params RegenModule[] modules);

        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Variables;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interperter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public abstract Dictionary<string, object> UnpackedVariables(string code, Dictionary<string, object> variables = null, params RegenModule[] modules);

        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Variables;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="allShouldBeOfType">Will assert the following: output.Variables.Values.Should().AllBeOfType(allShouldBeOfType)</param>
        /// <param name="variables">Optional variables to be passed to the interperter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public abstract Dictionary<string, object> UnpackedVariables(string code, Type allShouldBeOfType, Dictionary<string, object> variables = null, params RegenModule[] modules);

        public static void Debug(InterpredCode output) {
            Console.WriteLine("Output:  -----------------");
            Console.WriteLine(output.Output);
            Console.WriteLine("Variables: ---------------");
            foreach (var kv in output.Variables) {
                Console.WriteLine($"{kv.Key}:\t\t{kv.Value}\t\t{(kv.Value is Data d ? d.Value : kv.Value)}");
            }
        }
    }
}