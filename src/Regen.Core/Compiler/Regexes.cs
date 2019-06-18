using System.Text.RegularExpressions;

namespace Regen.Compiler {
    public class Regexes {
        public const RegexOptions DefaultRegexOptions = RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant;

        public const string LoneEndBlock = @"(?<=[\s\t^]|^[\w\d]?)%(?=[\s\t$]|^[\w\d]?) | ^ [\s\t]+ % $";
    }
}