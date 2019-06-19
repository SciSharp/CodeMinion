using System.Text.RegularExpressions;

namespace Regen.Compiler {
    public class Regexes {
        public const RegexOptions DefaultRegexOptions = RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant;

        public static readonly char[] VariableNameValidSymbols = new char[] {'_'};
    }
}