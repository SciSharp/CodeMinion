using System;
using System.Linq;
using System.Text.RegularExpressions;
using Regen.Helpers;

namespace Regen.Compiler {
    public class Parser {
        /// <summary>
        ///     The regex options used to in parsing.
        /// </summary>

        public static string DefineMarker = "_REGEN";

        public static string DefineGlobalsMarker = "_REGEN_GLOBAL[S]*";

        public static string FrameRegex = $@"(?<=\#if\s{DefineMarker})[\n\r]{{1,2}}    ([\s|\S]*?)    \#else[\n\r]{{1,2}}     ([\s|\S]*?)   (?=\#endif)";

        public static string GlobalFrameRegex = $@"\#if\s{DefineGlobalsMarker}[\n\r]{{1,2}}     ([\s|\S]*?)    \#endif";

        public void Consume(ref string code) {
            code = Consume(code);
        }

        public string Consume(string code) {
            var framed = Regex.Matches(code, FrameRegex, Regexes.DefaultRegexOptions);
            int additionalOffset = 0;
            foreach (Match frame in framed) {
                var regenmatch = frame.Groups[1];
                var regencode = regenmatch.Value;
                var outputFrame = frame.Groups[2];
                var inter = new Interperter(code, regencode);
                var ret = inter.Interpret();
                code = code
                    .Remove(outputFrame.Index + additionalOffset, outputFrame.Length)
                    .Insert(outputFrame.Index + additionalOffset, ret.Output);
                additionalOffset += ret.Output.Length - outputFrame.Length;
            }

            return code;
        }

        public void Consume(ref string code, int specificAtIndex) {
            code = Consume(code, specificAtIndex);
        }

        public string Consume(string code, int specificAtIndex) {
            var framed = Regex.Matches(code, FrameRegex, Regexes.DefaultRegexOptions);
            int additionalOffset = 0;
            foreach (Match frame in framed.Cast<Match>().Where(m=>m.DoesMatchNests(specificAtIndex))) {
                var regenmatch = frame.Groups[1];
                var regencode = regenmatch.Value;
                var outputFrame = frame.Groups[2];
                var inter = new Interperter(code, regencode);
                var ret = inter.Interpret();
                code = code
                    .Remove(outputFrame.Index + additionalOffset, outputFrame.Length)
                    .Insert(outputFrame.Index + additionalOffset, ret.Output);
                additionalOffset += ret.Output.Length - outputFrame.Length;
            }

            return code;
        }
    }
}