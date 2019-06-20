using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Regen.Compiler {
    public class Regexes {
        public const RegexOptions DefaultRegexOptions = RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant;

        public static readonly char[] VariableNameValidSymbols = new char[] {'_'};

        /// <summary>
        ///     Works only on single road.
        /// </summary>
        public static string DictionaryElementsRegex = @"\[ ((?:\[?([\s\S]+?),([\s\S]+?)\])) \|?";
    }
}