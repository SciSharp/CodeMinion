using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Regen.Helpers {
    public static class RegexExtensions {
        /// <summary>
        ///     Checks if <see cref="needle"/> is nested inside <see cref="haystack"/>.
        /// </summary>
        /// <param name="haystack">The <see cref="Match"/> that is suppose to contain <see cref="needle"/></param>
        /// <param name="needle">The <see cref="Match"/> that is supposed to be contained in <see cref="haystack"/></param>
        public static bool IsMatchNestedTo(this Match haystack, Match needle) {
            if (!haystack.Success || !needle.Success)
                return false;
            var middle = needle.Index + needle.Length / 2;
            return haystack.Index <= middle && middle <= haystack.Index + haystack.Length-1;
        }

        /// <summary>
        ///     Checks if <see cref="index"/> is nested inside <see cref="haystack"/>.
        /// </summary>
        /// <param name="haystack">The <see cref="Match"/> that is suppose to contain <see cref="index"/></param>
        /// <param name="index">The <see cref="int"/> index that is supposed to be inside of <see cref="haystack"/></param>
        public static bool DoesMatchNests(this Match haystack, int index) {
            if (!haystack.Success)
                return false;
            var middle = index;
            return haystack.Index <= middle && middle <= haystack.Index + haystack.Length;
        }

        /// <summary>
        ///     Out
        /// </summary>
        /// <param name="match"></param>
        /// <remarks>https://stackoverflow.com/a/27444808/1481186</remarks>
        public static void DisplayMatchResults(this Match match) {
            Console.WriteLine("Match has {0} captures", match.Captures.Count);

            int groupNo = 0;
            foreach (Group mm in match.Groups) {
                Console.WriteLine("  Group {0,2} has {1,2} captures '{2}'", groupNo, mm.Captures.Count, mm.Value);

                int captureNo = 0;
                foreach (Capture cc in mm.Captures) {
                    Console.WriteLine("       Capture {0,2} '{1}'", captureNo, cc);
                    captureNo++;
                }

                groupNo++;
            }

            groupNo = 0;
            foreach (Group mm in match.Groups) {
                Console.WriteLine("    match.Groups[{0}].Value == \"{1}\"", groupNo, match.Groups[groupNo].Value); //**
                groupNo++;
            }

            groupNo = 0;
            foreach (Group mm in match.Groups) {
                int captureNo = 0;
                foreach (Capture cc in mm.Captures) {
                    Console.WriteLine("    match.Groups[{0}].Captures[{1}].Value == \"{2}\"", groupNo, captureNo, match.Groups[groupNo].Captures[captureNo].Value); //**
                    captureNo++;
                }

                groupNo++;
            }
        }
    }
}