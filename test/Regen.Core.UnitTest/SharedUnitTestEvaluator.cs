using System;
using System.Collections.Generic;
using System.Linq;
using Regen.Compiler;
using Regen.Compiler.Expressions;
using Regen.DataTypes;
using Regen.Flee.PublicTypes;
using Regen.Helpers;
using Regen.Parser;
using Regen.Parser.Expressions;

namespace Regen.Core.Tests {
    public abstract class SharedUnitTestEvaluator {
        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Output;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interpreter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public abstract ParsedCode Parse(string code, Dictionary<string, object> variables = null, params RegenModule[] modules);

        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Variables;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interperter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public abstract VariableCollection Variables(string code, Dictionary<string, object> variables = null, params RegenModule[] modules);

        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Variables;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interperter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public abstract (ParsedCode ParsedCode, string Output) Compile(string code, Dictionary<string, object> variables = null, params RegenModule[] modules);

        /// <summary>
        ///     Runs the following code: return new Interperter(code, code).Run().Variables;
        /// </summary>
        /// <param name="code">The input code to compile</param>
        /// <param name="variables">Optional variables to be passed to the interperter</param>
        /// <param name="modules">The modules to include into the interpreter</param>
        public abstract Dictionary<string, object> UnpackedVariables(string code, Dictionary<string, object> variables = null, params RegenModule[] modules);

        public static void Debug(string output, VariableCollection variables) {
            Console.WriteLine("Output:  -----------------");
            Console.WriteLine(output);
            Console.WriteLine("Variables: ---------------");
            foreach (var kv in variables) {
                Console.WriteLine($"{kv.Key}:\t\t{kv.Value}\t\t{(kv.Value is Data d ? d.Value : kv.Value)}");
            }
        }

        public static void Debug(ParsedCode parsed) {
            Console.WriteLine("ParsedCode Output:  -----------------");
            Console.WriteLine(parsed.Output.Combine(parsed.Options));
            Console.WriteLine("ParsedCode Variables: ---------------");
            foreach (var kv in parsed.ParseActions.Where(act => act.Token == ParserToken.Declaration)) {
                var expr = (VariableDeclarationExpression) kv.Related.First();
                Console.WriteLine($"{expr.Name}:\t\t{expr.Right.ToString()}");
            }
        }
    }
}