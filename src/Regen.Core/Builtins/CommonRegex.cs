using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Threading;
using Regen.Compiler;
using Regen.DataTypes;

namespace Regen.Builtins {

    #region MyRegion

    public class RegexFactory {
        public static CommonRegex regex(string pattern) {
            return new CommonRegex(pattern);
        }
    }

    public class CommonRegex : IRegenModule {
        private Regex regex;

        public CommonRegex(string pattern) {
            regex = new Regex(pattern);
        }

        /// <summary>Returns an array of capturing group names for the regular expression.</summary>
        /// <returns>A string array of group names.</returns>
        public string[] GetGroupNames() {
            return regex.GetGroupNames();
        }

        /// <summary>Returns an array of capturing group numbers that correspond to group names in an array.</summary>
        /// <returns>An integer array of group numbers.</returns>
        public int[] GetGroupNumbers() {
            return regex.GetGroupNumbers();
        }

        /// <summary>Gets the group name that corresponds to the specified group number.</summary>
        /// <param name="i">The group number to convert to the corresponding group name. </param>
        /// <returns>A string that contains the group name associated with the specified group number. If there is no group name that corresponds to <paramref name="i" />, the method returns <see cref="F:System.String.Empty" />.</returns>
        public string GroupNameFromNumber(int i) {
            return regex.GroupNameFromNumber(i);
        }

        /// <summary>Returns the group number that corresponds to the specified group name.</summary>
        /// <param name="name">The group name to convert to the corresponding group number. </param>
        /// <returns>The group number that corresponds to the specified group name, or -1 if <paramref name="name" /> is not a valid group name.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name" /> is <see langword="null" />.</exception>
        public int GroupNumberFromName(string name) {
            return regex.GroupNumberFromName(name);
        }

        /// <summary>Indicates whether the regular expression specified in the <see cref="T:System.Text.RegularExpressions.Regex" /> constructor finds a match in a specified input string.</summary>
        /// <param name="input">The string to search for a match. </param>
        /// <returns>
        /// <see langword="true" /> if the regular expression finds a match; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.Text.RegularExpressions.RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section.</exception>
        public bool IsMatch(string input) {
            return regex.IsMatch(input);
        }

        /// <summary>Indicates whether the regular expression specified in the <see cref="T:System.Text.RegularExpressions.Regex" /> constructor finds a match in the specified input string, beginning at the specified starting position in the string.</summary>
        /// <param name="input">The string to search for a match. </param>
        /// <param name="startat">The character position at which to start the search. </param>
        /// <returns>
        /// <see langword="true" /> if the regular expression finds a match; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="startat" /> is less than zero or greater than the length of <paramref name="input" />.</exception>
        /// <exception cref="T:System.Text.RegularExpressions.RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section.</exception>
        public bool IsMatch(string input, int startat) {
            return regex.IsMatch(input, startat);
        }

        /// <summary>Searches the specified input string for the first occurrence of the regular expression specified in the <see cref="T:System.Text.RegularExpressions.Regex" /> constructor.</summary>
        /// <param name="input">The string to search for a match. </param>
        /// <returns>An object that contains information about the match.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.Text.RegularExpressions.RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section.</exception>
        public Match Match(string input) {
            return regex.Match(input);
        }

        /// <summary>Searches the input string for the first occurrence of a regular expression, beginning at the specified starting position in the string.</summary>
        /// <param name="input">The string to search for a match. </param>
        /// <param name="startat">The zero-based character position at which to start the search. </param>
        /// <returns>An object that contains information about the match.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="startat" /> is less than zero or greater than the length of <paramref name="input" />.</exception>
        /// <exception cref="T:System.Text.RegularExpressions.RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section.</exception>
        public Match Match(string input, int startat) {
            return regex.Match(input, startat);
        }

        /// <summary>Searches the input string for the first occurrence of a regular expression, beginning at the specified starting position and searching only the specified number of characters.</summary>
        /// <param name="input">The string to search for a match. </param>
        /// <param name="beginning">The zero-based character position in the input string that defines the leftmost position to be searched. </param>
        /// <param name="length">The number of characters in the substring to include in the search. </param>
        /// <returns>An object that contains information about the match.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="beginning" /> is less than zero or greater than the length of <paramref name="input" />.-or-
        /// <paramref name="length" /> is less than zero or greater than the length of <paramref name="input" />.-or-
        /// <paramref name="beginning" /><see langword="+" /><paramref name="length" /><see langword="–1" /> identifies a position that is outside the range of <paramref name="input" />.</exception>
        /// <exception cref="T:System.Text.RegularExpressions.RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section.</exception>
        public Match Match(string input, int beginning, int length) {
            return regex.Match(input, beginning, length);
        }

        /// <summary>Searches the specified input string for all occurrences of a regular expression, beginning at the specified starting position in the string.</summary>
        /// <param name="input">The string to search for a match. </param>
        /// <param name="startat">The character position in the input string at which to start the search. </param>
        /// <returns>A collection of the <see cref="T:System.Text.RegularExpressions.Match" /> objects found by the search. If no matches are found, the method returns an empty collection object.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="startat" /> is less than zero or greater than the length of <paramref name="input" />.</exception>
        public MatchCollection Matches(string input, int startat) {
            return regex.Matches(input, startat);
        }

        /// <summary>Searches the specified input string for all occurrences of a regular expression.</summary>
        /// <param name="input">The string to search for a match.</param>
        /// <returns>A collection of the <see cref="T:System.Text.RegularExpressions.Match" /> objects found by the search. If no matches are found, the method returns an empty collection object.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is <see langword="null" />.</exception>
        public MatchCollection Matches(string input) {
            return regex.Matches(input);
        }

        /// <summary>Gets the time-out interval of the current instance.</summary>
        /// <returns>The maximum time interval that can elapse in a pattern-matching operation before a <see cref="T:System.Text.RegularExpressions.RegexMatchTimeoutException" /> is thrown, or <see cref="F:System.Text.RegularExpressions.Regex.InfiniteMatchTimeout" /> if time-outs are disabled.</returns>
        public TimeSpan MatchTimeout => regex.MatchTimeout;

        /// <summary>Gets the options that were passed into the <see cref="T:System.Text.RegularExpressions.Regex" /> constructor.</summary>
        /// <returns>One or more members of the <see cref="T:System.Text.RegularExpressions.RegexOptions" /> enumeration that represent options that were passed to the <see cref="T:System.Text.RegularExpressions.Regex" /> constructor </returns>
        public RegexOptions Options => regex.Options;

        /// <summary>In a specified input string, replaces all strings that match a regular expression pattern with a specified replacement string. </summary>
        /// <param name="input">The string to search for a match. </param>
        /// <param name="replacement">The replacement string. </param>
        /// <returns>A new string that is identical to the input string, except that the replacement string takes the place of each matched string. If the regular expression pattern is not matched in the current instance, the method returns the current instance unchanged. </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> or <paramref name="replacement" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.Text.RegularExpressions.RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section.</exception>
        public string Replace(string input, string replacement) {
            return regex.Replace(input, replacement);
        }

        /// <summary>In a specified input string, replaces a specified maximum number of strings that match a regular expression pattern with a specified replacement string. </summary>
        /// <param name="input">The string to search for a match. </param>
        /// <param name="replacement">The replacement string. </param>
        /// <param name="count">The maximum number of times the replacement can occur. </param>
        /// <returns>A new string that is identical to the input string, except that the replacement string takes the place of each matched string. If the regular expression pattern is not matched in the current instance, the method returns the current instance unchanged.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> or <paramref name="replacement" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.Text.RegularExpressions.RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section.</exception>
        public string Replace(string input, string replacement, int count) {
            return regex.Replace(input, replacement, count);
        }

        /// <summary>In a specified input substring, replaces a specified maximum number of strings that match a regular expression pattern with a specified replacement string. </summary>
        /// <param name="input">The string to search for a match. </param>
        /// <param name="replacement">The replacement string. </param>
        /// <param name="count">Maximum number of times the replacement can occur. </param>
        /// <param name="startat">The character position in the input string where the search begins. </param>
        /// <returns>A new string that is identical to the input string, except that the replacement string takes the place of each matched string. If the regular expression pattern is not matched in the current instance, the method returns the current instance unchanged. </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> or <paramref name="replacement" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="startat" /> is less than zero or greater than the length of <paramref name="input" />.</exception>
        /// <exception cref="T:System.Text.RegularExpressions.RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section.</exception>
        public string Replace(string input, string replacement, int count, int startat) {
            return regex.Replace(input, replacement, count, startat);
        }

        /// <summary>In a specified input string, replaces all strings that match a specified regular expression with a string returned by a <see cref="T:System.Text.RegularExpressions.MatchEvaluator" /> delegate. </summary>
        /// <param name="input">The string to search for a match. </param>
        /// <param name="evaluator">A custom method that examines each match and returns either the original matched string or a replacement string.</param>
        /// <returns>A new string that is identical to the input string, except that a replacement string takes the place of each matched string. If the regular expression pattern is not matched in the current instance, the method returns the current instance unchanged. </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> or <paramref name="evaluator" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.Text.RegularExpressions.RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section.</exception>
        public string Replace(string input, MatchEvaluator evaluator) {
            return regex.Replace(input, evaluator);
        }

        /// <summary>In a specified input string, replaces a specified maximum number of strings that match a regular expression pattern with a string returned by a <see cref="T:System.Text.RegularExpressions.MatchEvaluator" /> delegate. </summary>
        /// <param name="input">The string to search for a match. </param>
        /// <param name="evaluator">A custom method that examines each match and returns either the original matched string or a replacement string.</param>
        /// <param name="count">The maximum number of times the replacement will occur. </param>
        /// <returns>A new string that is identical to the input string, except that a replacement string takes the place of each matched string. If the regular expression pattern is not matched in the current instance, the method returns the current instance unchanged. </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> or <paramref name="evaluator" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.Text.RegularExpressions.RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section.</exception>
        public string Replace(string input, MatchEvaluator evaluator, int count) {
            return regex.Replace(input, evaluator, count);
        }

        /// <summary>In a specified input substring, replaces a specified maximum number of strings that match a regular expression pattern with a string returned by a <see cref="T:System.Text.RegularExpressions.MatchEvaluator" /> delegate. </summary>
        /// <param name="input">The string to search for a match. </param>
        /// <param name="evaluator">A custom method that examines each match and returns either the original matched string or a replacement string.</param>
        /// <param name="count">The maximum number of times the replacement will occur. </param>
        /// <param name="startat">The character position in the input string where the search begins. </param>
        /// <returns>A new string that is identical to the input string, except that a replacement string takes the place of each matched string. If the regular expression pattern is not matched in the current instance, the method returns the current instance unchanged. </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> or <paramref name="evaluator" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="startat" /> is less than zero or greater than the length of <paramref name="input" />.</exception>
        /// <exception cref="T:System.Text.RegularExpressions.RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section.</exception>
        public string Replace(string input, MatchEvaluator evaluator, int count, int startat) {
            return regex.Replace(input, evaluator, count, startat);
        }

        /// <summary>Gets a value that indicates whether the regular expression searches from right to left.</summary>
        /// <returns>
        /// <see langword="true" /> if the regular expression searches from right to left; otherwise, <see langword="false" />.</returns>
        public bool RightToLeft => regex.RightToLeft;

        /// <summary>Splits an input string into an array of substrings at the positions defined by a regular expression pattern specified in the <see cref="T:System.Text.RegularExpressions.Regex" /> constructor.</summary>
        /// <param name="input">The string to split. </param>
        /// <returns>An array of strings.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.Text.RegularExpressions.RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section.</exception>
        public string[] Split(string input) {
            return regex.Split(input);
        }

        /// <summary>Splits an input string a specified maximum number of times into an array of substrings, at the positions defined by a regular expression specified in the <see cref="T:System.Text.RegularExpressions.Regex" /> constructor.</summary>
        /// <param name="input">The string to be split. </param>
        /// <param name="count">The maximum number of times the split can occur. </param>
        /// <returns>An array of strings.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.Text.RegularExpressions.RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section.</exception>
        public string[] Split(string input, int count) {
            return regex.Split(input, count);
        }

        /// <summary>Splits an input string a specified maximum number of times into an array of substrings, at the positions defined by a regular expression specified in the <see cref="T:System.Text.RegularExpressions.Regex" /> constructor. The search for the regular expression pattern starts at a specified character position in the input string.</summary>
        /// <param name="input">The string to be split. </param>
        /// <param name="count">The maximum number of times the split can occur. </param>
        /// <param name="startat">The character position in the input string where the search will begin. </param>
        /// <returns>An array of strings.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="startat" /> is less than zero or greater than the length of <paramref name="input" />.</exception>
        /// <exception cref="T:System.Text.RegularExpressions.RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section.</exception>
        public string[] Split(string input, int count, int startat) {
            return regex.Split(input, count, startat);
        }

        /// <summary>
        ///     Simply implement as return this
        /// </summary>
        /// <remarks>Because <see cref="Flee"/> implements external modules as a namespace and not an actual type, returns self </remarks>
        public Data Self() {
            return null; //TODO add ObjectScalar.
        }
    }

    #endregion


}