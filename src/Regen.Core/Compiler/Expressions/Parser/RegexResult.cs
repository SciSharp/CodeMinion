using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Regen.Compiler.Expressions {

    [DebuggerDisplay("Idx{Index} Len{Length} | {Value}")]
    public class RegexResult {
        public string Value { get; set; }
        public GroupCollection Groups { get; set; }
        public int Index { get; set; }
        public int Length { get; set; }
    }
}