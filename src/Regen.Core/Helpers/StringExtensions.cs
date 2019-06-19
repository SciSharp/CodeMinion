using System;

namespace Regen.Helpers {
    public static class StringExtensions {
        /// <summary>
        ///     Gets string between <see cref="left"/> and <see cref="right"/> (excluding both).
        /// </summary>
        /// <param name="haystack">The string to search</param>
        /// <param name="left">The left marker, can be empty or null</param>
        /// <param name="right">The right marker, can be empty or null</param>
        /// <param name="indexFrom">From what index to start.</param>
        /// <returns></returns>
        /// <remarks>https://stackoverflow.com/a/46940181/1481186</remarks>
        public static (string Content, int LeftIndex, int RightIndex) StringBetween(this string haystack, string left, string right, int indexFrom = 0) {
            right = right ?? "";
            left = left ?? "";
            int p1 = left == "" ? 0 : haystack.IndexOf(left, indexFrom, StringComparison.Ordinal) + left.Length;
            if (p1 == -1)
                return default;
            int p2 = haystack.IndexOf(right, p1, StringComparison.Ordinal);
            if (p2 == -1)
                return (haystack.Substring(p1), p1, -1);

            var content = string.IsNullOrEmpty(right) ? haystack.Substring(p1) : haystack.Substring(p1, p2 - p1);
            return (content, p1, p2);
        }        
        
        /// <summary>
        ///     Gets string between <see cref="left"/> and <see cref="right"/> (excluding both).
        /// </summary>
        /// <param name="haystack">The string to search</param>
        /// <param name="left">The left marker, can be empty or null</param>
        /// <param name="right">The right marker, can be empty or null</param>
        /// <param name="indexFrom">From what index to start.</param>
        /// <returns></returns>
        /// <remarks>https://stackoverflow.com/a/46940181/1481186</remarks>
        public static string RemoveStringBetween(this string haystack, string left, string right, int indexFrom = 0) {
            right = right ?? "";
            left = left ?? "";
            int p1 = left == "" ? 0 : haystack.IndexOf(left, indexFrom, StringComparison.Ordinal) + left.Length;
            if (p1 == -1)
                return default;
            int p2 = haystack.IndexOf(right, p1, StringComparison.Ordinal);
            if (p2 == -1)
                return haystack.Remove(0, p1);

            var content = string.IsNullOrEmpty(right) ? haystack.Remove(0,p1) : haystack.Remove(p1, p2 - p1);
            return content;
        }
    }
}