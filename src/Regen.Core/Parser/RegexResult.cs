using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Regen.Parser {

    /// <summary>
    ///     A wrapper around <see cref="Regex"/>'s <see cref="Match"/> since it has no public constructors.
    /// </summary>

    [DebuggerDisplay("Idx{Index} Len{Length} | {Value}")]
    public class RegexResult {
        public string Value { get; set; }
        public GroupCollection Groups { get; set; }
        public int Index { get; set; }
        public int Length { get; set; }
    }
}