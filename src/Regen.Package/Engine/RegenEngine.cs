using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Regen.Compiler;
using Regen.Helpers;
using Regen.Parser;

namespace Regen.Engine {
    /// <summary>
    ///     Central class to different types of methods that use <see cref="Regen"/>.<br></br>
    ///     Knows to handle _REGEN and _REGEN_GLOBAL
    /// </summary>
    public static class RegenEngine {
        /// <summary>
        ///     All global files contents that were found.
        /// </summary>
        public static List<string> Globals { get; } = new List<string>();

        /// <summary>
        ///     Compiles the entire file _REGEN frames and returns the result after the parsed code was inserted to the #ELSE block.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string Compile(string code) {
            foreach (var frame in RegenEngine.CompileFrame(code).Reverse())
                frame.ApplyChanges(ref code);

            return code;
        }

        public static CodeFrame[] CompileFrame(string code) {
            return CodeFrame.Create(code)
                .Select(frame => CompileFrame(frame, code))
                .OrderBy(f => f.Match.Index)
                .ToArray();
        }

        public static CodeFrame ParseAt(string code, int index) {
            return CompileFrame(CodeFrame.Create(code, index), code);
        }

        public static CodeFrame CompileFrame(CodeFrame frame, string code) {
            var compiler = new RegenCompiler(); //todo modules here?
            var parsedCode = ExpressionParser.Parse(frame.Input);
            LoadGlobals(compiler);
            //handle globals
            var globals = CodeFrame.CreateGlobals(code);
            foreach (var globalFrame in globals) {
                compiler.CompileGlobal(globalFrame.Input);
            }

            frame.Output = compiler.Compile(parsedCode);
            return frame;
        }

        static void LoadGlobals(RegenCompiler compiler) {
            foreach (var content in Globals) {
                compiler.CompileGlobal(content);
            }
        }
    }

    /// <summary>
    ///     A "frame" that contains the input code in <see cref="Input"/> and Output
    /// </summary>
    public class CodeFrame {
        public const string FrameRegex = @"\#if\s_REGEN(?:\s)*[\n\r]{1,2}    ([\s|\S]*?)    [\n\r]{1,2} \#else(?:\s)* [\n\r]{1,2}   ([\s|\S]*?)?   \#endif";
        public const string LoneFrameRegex = @"\#if\s_REGEN_GLOBAL(?:\s)*[\n\r]{1,2}    ([\s|\S]*?)    [\n\r]{1,2} (?:\#endif)+?";

        /// <summary>
        ///     The match that was used to create this frame.
        /// </summary>
        public Match Match { get; set; }

        /// <summary>
        ///     The code between #if _REGEN and #else
        /// </summary>
        public string Input { get; set; }

        /// <summary>
        ///     The code between #else and #endif
        /// </summary>
        public string Output { get; set; }

        /// <summary>
        ///     Is it a lone frame without an Output
        /// </summary>
        public bool IsLone { get; set; }

        private CodeFrame(Match match) {
            if (match == null || !match.Success)
                throw new ArgumentException("Match must be successful.", nameof(match));

            Match = match;
            Input = match.Groups[1].Value;
            if (match.Groups.Count == 2) {
                IsLone = true;
            } else if (match.Groups.Count > 2) {
                Output = match.Groups[2].Value;
            }
        }

        public static CodeFrame[] Create(string fileContents) {
            var matches = Regex.Matches(fileContents, FrameRegex, Regexes.DefaultRegexOptions).Cast<Match>().ToArray();
            if (matches.Length == 0) {
                //fallback to lone
                matches = Regex.Matches(fileContents, LoneFrameRegex, Regexes.DefaultRegexOptions).Cast<Match>().Where(m => !m.Groups[1].Value.Contains("#else")).ToArray();
                if (matches.Length == 0) {
                    return Array.Empty<CodeFrame>();
                }
            }

            return matches.Select(m => new CodeFrame(m)).ToArray();
        }

        public static CodeFrame[] CreateGlobals(string fileContents) {
            var matches = Regex.Matches(fileContents, LoneFrameRegex, Regexes.DefaultRegexOptions).Cast<Match>().ToArray();
            if (matches.Length == 0) {
                //fallback to lone
                matches = Regex.Matches(fileContents, LoneFrameRegex, Regexes.DefaultRegexOptions).Cast<Match>().Where(m => !m.Groups[1].Value.Contains("#else")).ToArray();
                if (matches.Length == 0) {
                    return Array.Empty<CodeFrame>();
                }
            }

            return matches.Select(m => new CodeFrame(m)).ToArray();
        }

        public static CodeFrame[] Create(string fileContents, string keyword) {
            var matches = Regex.Matches(fileContents, LoneFrameRegex.Replace("_REGEN_GLOBAL", Regex.Escape(keyword)), Regexes.DefaultRegexOptions).Cast<Match>().ToArray();
            if (matches.Length == 0) {
                //fallback to lone
                matches = Regex.Matches(fileContents, LoneFrameRegex.Replace("_REGEN_GLOBAL", Regex.Escape(keyword)), Regexes.DefaultRegexOptions).Cast<Match>().Where(m => !m.Groups[1].Value.Contains("#else")).ToArray();
                if (matches.Length == 0) {
                    return Array.Empty<CodeFrame>();
                }
            }

            return matches.Select(m => new CodeFrame(m)).ToArray();
        }

        public static CodeFrame Create(string fileContents, int specificIndex) {
            var matches = Regex.Matches(fileContents, FrameRegex, Regexes.DefaultRegexOptions).Cast<Match>().ToArray();
            var match = matches.FirstOrDefault(m => m.DoesMatchNests(specificIndex));
            if (match == null) {
                //fallback to lone
                match = Regex.Matches(fileContents, LoneFrameRegex, Regexes.DefaultRegexOptions).Cast<Match>().Where(m => !m.Groups[1].Value.Contains("#else")).FirstOrDefault(m => m.DoesMatchNests(specificIndex));
                if (match == null) {
                    return null;
                }
            }

            return new CodeFrame(match);
        }

        public void ApplyChanges(ref string target) {
            if (this.IsLone)
                throw new NotSupportedException("Unable to apply changes when using a lone target.");
            var indx = Match.Groups[2].Index;
            var cnt = Match.Groups[2].Length;
            target = target.Remove(indx, cnt)
                .Insert(indx, Output);
        }
    }
}