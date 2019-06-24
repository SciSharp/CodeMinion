using System.Collections.Generic;
using System.Diagnostics;

namespace Regen.Compiler {
    [DebuggerNonUserCode]
    public class InterpreterOptions {
        public static List<string> BuiltinKeywords = new List<string>() {"i"};

        /// <summary>
        ///     At the end of compilation, looks for % that seems to be alone in the row and removes them.
        /// </summary>
        public bool ClearLoneBlockmarkers { get; set; }

        /// <summary>
        ///     Unescapes any marks of  \% -> %  in the output code.
        /// </summary>
        public bool UnespacePrecentages { get; set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        [DebuggerNonUserCode]
        public InterpreterOptions() {
            ClearLoneBlockmarkers = true;
            UnespacePrecentages = true;
        }
    }
}