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
        /// <summary>
        /// (Regen.Core.Generated), Version=0.0.1.3, Culture=neutral, PublicKeyToken=c27591280ed7de68
        /// </summary>
        public static string SelectNamespaceFromAssemblyName = @"^((?:[\s\S])+?[^,]+)";
    }
}