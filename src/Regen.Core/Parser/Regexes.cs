using System.Text.RegularExpressions;

namespace Regen.Parser {
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

        public static string Parenthases = @"\((?:[^()]|(?<open>\()|(?<-open>\)))+(?(open)(?!))\)";

        public static string DefineMarker = "_REGEN";

        public static string DefineGlobalsMarker = "_REGEN_GLOBAL[S]*";

        public static string FrameRegex = $@"(?<=\#if\s{DefineMarker})[\n\r]{{1,2}}    ([\s|\S]*?)    \#else[\n\r]{{1,2}}     ([\s|\S]*?)   (?=\#endif)";

        public static string GlobalFrameRegex = $@"\#if\s{DefineGlobalsMarker}[\n\r]{{1,2}}     ([\s|\S]*?)    \#endif";

        public static string TemplateFrameRegex = @"\#if\s_REGEN_TEMPLATE[\s\S]+?\#endif[\n\r]{0,2}";
    }
}