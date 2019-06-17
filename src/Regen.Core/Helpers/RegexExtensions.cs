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
            return haystack.Index <= middle && middle <= haystack.Index + haystack.Length;
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
    }
}