using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.Versioning;
using System.Text;

namespace Regen.Compiler.Helpers {
    public abstract class StringSpan : ICloneable {
        #region Static

        public static StringSpan Create(string str) {
            return new StringSource(str);
        }

        public static StringSpan Create(IEnumerable<char> str) {
            return new StringSource(str);
        }

        public static StringSpan Empty { get; } = new StringSource("");

        public static StringSpan Join(String separator, IEnumerable<StringSpan> values) {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            Contract.Ensures(Contract.Result<String>() != null);
            Contract.EndContractBlock();

            if (separator == null)
                separator = String.Empty;

            using (IEnumerator<StringSpan> en = values.GetEnumerator()) {
                if (!en.MoveNext())
                    return StringSpan.Empty;

                var sb = new StringSource();
                if (en.Current != null) {
                    sb.Add(en.Current.ToCharArray());
                }

                while (en.MoveNext()) {
                    sb.Add(separator);
                    if (en.Current != null) {
                        sb.Add(en.Current.ToCharArray());
                    }
                }

                return sb;
            }
        }

        #endregion

        public abstract int Length { get; }

        public abstract char this[int index] { get; set; }

        /// <summary>
        ///     The index the string span begins (inclusive)
        /// </summary>
        public abstract int Start { get; }

        /// <summary>
        ///     The index the string span ends (inclusive)
        /// </summary>
        public abstract int End { get; }

        /// <summary>
        ///     The characters this string holds.
        /// </summary>
        public abstract List<char> Chars { get; protected set; }

        public abstract void Add(string str);
        public abstract void Add(IEnumerable<char> chars);
        public abstract StringSlice Substring(int index, int length);
        public abstract StringSlice Substring(Range range);
        public abstract int Remove(int index);
        public abstract bool RemoveAt(int index);
        public abstract int Remove(int index, int length);
        public abstract int Remove(Range range);
        public abstract void Insert(int index, params char[] chars);
        public abstract void Insert(int index, string str);

        /// <summary>
        ///     Sequence of removing and then placing without making slices to turn deleted.
        /// </summary>
        public abstract void ExchangeAt(int index, int endindex, string place);

        /// <summary>
        ///     Sequence of removing and then placing without making slices to turn deleted.
        /// </summary>
        public abstract void ExchangeAt(int index, int endindex, IEnumerable<char> chars);

        /// <summary>
        ///     Sequence of removing and then placing without making slices to turn deleted.
        /// </summary>
        public abstract void ExchangeAt(int index, string place);

        public abstract int TrimEnd(params char[] chars);
        public abstract int TrimStart(params char[] chars);
        public abstract int Trim(params char[] chars);
        public abstract bool Contains(IList<char> niddle);
        public abstract bool Contains(string niddle);
        public abstract bool Contains(StringSpan niddle);
        public abstract char[] ToCharArray();
        public abstract StringSpan[] Split(string separator, StringSplitOptions options);
        public abstract StringSpan[] Split(char @char, StringSplitOptions options);

        public abstract void ReplaceWith(string place);

        public abstract void ReplaceWith(IEnumerable<char> chars);

        /// <summary>
        ///     Tests if given <see cref="index"/> is a valid index to access this <see cref="StringSpan"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract bool IsIndexInside(int index);

        /// <summary>
        ///     Add more characters to the end of this string
        /// </summary>
        /// <param name="chars">Number of characters (1 based) to add</param>
        /// <param name="fill">If it is the end of the string, <paramref name="fill"/> will be used as value.</param>
        public abstract void Extend(int chars, char fill = '\0');

        // Determines the position within this string of the first occurence of the specified
        // string, according to the specified search criteria.  The search begins at
        // the first character of this string, it is case-sensitive and ordinal (code-point)
        // comparison is used.
        //
        [Pure]
        public int IndexOf(String value) {
            return IndexOf(value, StringComparison.CurrentCulture);
        }

        // Determines the position within this string of the first occurence of the specified
        // string, according to the specified search criteria.  The search begins at
        // startIndex, it is case-sensitive and ordinal (code-point) comparison is used.
        //
        [Pure]
        public int IndexOf(String value, int startIndex) {
            return IndexOf(value, startIndex, StringComparison.CurrentCulture);
        }

        // Determines the position within this string of the first occurence of the specified
        // string, according to the specified search criteria.  The search begins at
        // startIndex, ends at endIndex and ordinal (code-point) comparison is used.
        //
        [Pure]
        public int IndexOf(String value, int startIndex, int count) {
            if (startIndex < 0 || startIndex > this.Length) {
                throw new ArgumentOutOfRangeException("startIndex", ("ArgumentOutOfRange_Index"));
            }

            if (count < 0 || count > this.Length - startIndex) {
                throw new ArgumentOutOfRangeException("count", ("ArgumentOutOfRange_Count"));
            }

            Contract.EndContractBlock();

            return IndexOf(value, startIndex, count, StringComparison.CurrentCulture);
        }

        [Pure]
        public int IndexOf(String value, StringComparison comparisonType) {
            return IndexOf(value, 0, this.Length, comparisonType);
        }

        [Pure]
        public int IndexOf(String value, int startIndex, StringComparison comparisonType) {
            return IndexOf(value, startIndex, this.Length - startIndex, comparisonType);
        }

        [Pure]
        [System.Security.SecuritySafeCritical]
        public int IndexOf(String value, int startIndex, int count, StringComparison comparisonType) {
            // Validate inputs
            if (value == null)
                throw new ArgumentNullException("value");

            if (startIndex < 0 || startIndex > this.Length)
                throw new ArgumentOutOfRangeException("startIndex", ("ArgumentOutOfRange_Index"));

            if (count < 0 || startIndex > this.Length - count)
                throw new ArgumentOutOfRangeException("count", ("ArgumentOutOfRange_Count"));
            Contract.EndContractBlock();
            var str = this.ToString();
            switch (comparisonType) {
                case StringComparison.CurrentCulture:
                    return CultureInfo.CurrentCulture.CompareInfo.IndexOf(str, value, startIndex, count, CompareOptions.None);

                case StringComparison.CurrentCultureIgnoreCase:
                    return CultureInfo.CurrentCulture.CompareInfo.IndexOf(str, value, startIndex, count, CompareOptions.IgnoreCase);

                case StringComparison.InvariantCulture:
                    return CultureInfo.InvariantCulture.CompareInfo.IndexOf(str, value, startIndex, count, CompareOptions.None);

                case StringComparison.InvariantCultureIgnoreCase:
                    return CultureInfo.InvariantCulture.CompareInfo.IndexOf(str, value, startIndex, count, CompareOptions.IgnoreCase);

                case StringComparison.Ordinal:
                    return CultureInfo.InvariantCulture.CompareInfo.IndexOf(str, value, startIndex, count, CompareOptions.Ordinal);

                case StringComparison.OrdinalIgnoreCase:
                    return CultureInfo.InvariantCulture.CompareInfo.IndexOf(this.ToString(), value, startIndex, count, CompareOptions.IgnoreCase);

                default:
                    throw new ArgumentException(("NotSupported_StringComparison"), nameof(comparisonType));
            }
        }

        // Returns the index of the last occurance of any character in value in the current instance.
        // The search starts at startIndex and runs to endIndex. [startIndex,endIndex].
        // The character at position startIndex is included in the search.  startIndex is the larger
        // index within the string.
        //
        [Pure]
        public int LastIndexOf(String value) {
            return LastIndexOf(value, this.Length - 1, this.Length, StringComparison.CurrentCulture);
        }

        [Pure]
        public int LastIndexOf(String value, int startIndex) {
            return LastIndexOf(value, startIndex, startIndex + 1, StringComparison.CurrentCulture);
        }

        [Pure]
        public int LastIndexOf(String value, int startIndex, int count) {
            if (count < 0) {
                throw new ArgumentOutOfRangeException(nameof(count), ("ArgumentOutOfRange_Count"));
            }

            Contract.EndContractBlock();

            return LastIndexOf(value, startIndex, count, StringComparison.CurrentCulture);
        }

        [Pure]
        public int LastIndexOf(String value, StringComparison comparisonType) {
            return LastIndexOf(value, this.Length - 1, this.Length, comparisonType);
        }

        [Pure]
        public int LastIndexOf(String value, int startIndex, StringComparison comparisonType) {
            return LastIndexOf(value, startIndex, startIndex + 1, comparisonType);
        }

        [Pure]
        [System.Security.SecuritySafeCritical]
        public int LastIndexOf(String value, int startIndex, int count, StringComparison comparisonType) {
            if (value == null)
                throw new ArgumentNullException("value");
            Contract.EndContractBlock();

            // Special case for 0 length input strings
            if (this.Length == 0 && (startIndex == -1 || startIndex == 0))
                return (value.Length == 0) ? 0 : -1;

            // Now after handling empty strings, make sure we're not out of range
            if (startIndex < 0 || startIndex > this.Length)
                throw new ArgumentOutOfRangeException("startIndex", ("ArgumentOutOfRange_Index"));

            // Make sure that we allow startIndex == this.Length
            if (startIndex == this.Length) {
                startIndex--;
                if (count > 0)
                    count--;

                // If we are looking for nothing, just return 0
                if (value.Length == 0 && count >= 0 && startIndex - count + 1 >= 0)
                    return startIndex;
            }

            // 2nd half of this also catches when startIndex == MAXINT, so MAXINT - 0 + 1 == -1, which is < 0.
            if (count < 0 || startIndex - count + 1 < 0)
                throw new ArgumentOutOfRangeException("count", ("ArgumentOutOfRange_Count"));

            var str = this.ToString();
            switch (comparisonType) {
                case StringComparison.CurrentCulture:
                    return CultureInfo.CurrentCulture.CompareInfo.LastIndexOf(str, value, startIndex, count, CompareOptions.None);

                case StringComparison.CurrentCultureIgnoreCase:
                    return CultureInfo.CurrentCulture.CompareInfo.LastIndexOf(str, value, startIndex, count, CompareOptions.IgnoreCase);

                case StringComparison.InvariantCulture:
                    return CultureInfo.InvariantCulture.CompareInfo.LastIndexOf(str, value, startIndex, count, CompareOptions.None);

                case StringComparison.InvariantCultureIgnoreCase:
                    return CultureInfo.InvariantCulture.CompareInfo.LastIndexOf(str, value, startIndex, count, CompareOptions.IgnoreCase);
                case StringComparison.Ordinal:
                    return CultureInfo.InvariantCulture.CompareInfo.LastIndexOf(str, value, startIndex, count, CompareOptions.Ordinal);

                case StringComparison.OrdinalIgnoreCase:
                    return CultureInfo.InvariantCulture.CompareInfo.LastIndexOf(str, value, startIndex, count, CompareOptions.IgnoreCase);
                default:
                    throw new ArgumentException(("NotSupported_StringComparison"), "comparisonType");
            }
        }

        [Pure]
        public Boolean StartsWith(String value) {
            if ((Object) value == null) {
                throw new ArgumentNullException("value");
            }

            Contract.EndContractBlock();
            return StartsWith(value, StringComparison.CurrentCulture);
        }

        [Pure]
        [System.Security.SecuritySafeCritical] // auto-generated
        public Boolean StartsWith(String value, StringComparison comparisonType) {
            if ((Object) value == null) {
                throw new ArgumentNullException("value");
            }

            if (comparisonType < StringComparison.CurrentCulture || comparisonType > StringComparison.OrdinalIgnoreCase) {
                throw new ArgumentException(("NotSupported_StringComparison"), "comparisonType");
            }

            Contract.EndContractBlock();

            if ((Object) this == (Object) value) {
                return true;
            }

            if (value.Length == 0) {
                return true;
            }

            var str = this.ToString();
            switch (comparisonType) {
                case StringComparison.CurrentCulture:
                    return CultureInfo.CurrentCulture.CompareInfo.IsPrefix(str, value, CompareOptions.None);
                case StringComparison.CurrentCultureIgnoreCase:
                    return CultureInfo.CurrentCulture.CompareInfo.IsPrefix(str, value, CompareOptions.IgnoreCase);
                case StringComparison.InvariantCulture:
                case StringComparison.Ordinal:
                    return CultureInfo.InvariantCulture.CompareInfo.IsPrefix(str, value, CompareOptions.None);
                case StringComparison.OrdinalIgnoreCase:
                case StringComparison.InvariantCultureIgnoreCase:
                    return CultureInfo.InvariantCulture.CompareInfo.IsPrefix(str, value, CompareOptions.IgnoreCase);
                default:
                    throw new ArgumentException(("NotSupported_StringComparison"), nameof(comparisonType));
            }
        }

        [Pure]
        public Boolean StartsWith(String value, Boolean ignoreCase, CultureInfo culture) {
            if (null == value) {
                throw new ArgumentNullException("value");
            }

            Contract.EndContractBlock();

            if ((object) this == (object) value) {
                return true;
            }

            CultureInfo referenceCulture;
            if (culture == null)
                referenceCulture = CultureInfo.CurrentCulture;
            else
                referenceCulture = culture;

            return referenceCulture.CompareInfo.IsPrefix(this.ToString(), value, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
        }


        // Determines whether a specified string is a suffix of the the current instance.
        //
        // The case-sensitive and culture-sensitive option is set by options,
        // and the default culture is used.
        //        
        [Pure]
        public Boolean EndsWith(String value) {
            return EndsWith(value, StringComparison.CurrentCulture);
        }

        [Pure]
        [System.Security.SecuritySafeCritical] // auto-generated
        public Boolean EndsWith(String value, StringComparison comparisonType) {
            if ((Object) value == null) {
                throw new ArgumentNullException("value");
            }

            if (comparisonType < StringComparison.CurrentCulture || comparisonType > StringComparison.OrdinalIgnoreCase) {
                throw new ArgumentException(("NotSupported_StringComparison"), "comparisonType");
            }

            Contract.EndContractBlock();

            if ((Object) this == (Object) value) {
                return true;
            }

            if (value.Length == 0) {
                return true;
            }

            var str = this.ToString();
            switch (comparisonType) {
                case StringComparison.CurrentCulture:
                    return CultureInfo.CurrentCulture.CompareInfo.IsSuffix(str, value, CompareOptions.None);

                case StringComparison.CurrentCultureIgnoreCase:
                    return CultureInfo.CurrentCulture.CompareInfo.IsSuffix(str, value, CompareOptions.IgnoreCase);

                case StringComparison.Ordinal:
                case StringComparison.InvariantCulture:
                    return CultureInfo.InvariantCulture.CompareInfo.IsSuffix(str, value, CompareOptions.None);

                case StringComparison.OrdinalIgnoreCase:
                case StringComparison.InvariantCultureIgnoreCase:
                    return CultureInfo.InvariantCulture.CompareInfo.IsSuffix(str, value, CompareOptions.IgnoreCase);
                default:
                    throw new ArgumentException(("NotSupported_StringComparison"), "comparisonType");
            }
        }

        [Pure]
        public Boolean EndsWith(String value, Boolean ignoreCase, CultureInfo culture) {
            if (null == value) {
                throw new ArgumentNullException("value");
            }

            Contract.EndContractBlock();

            if ((object) this == (object) value) {
                return true;
            }

            CultureInfo referenceCulture;
            if (culture == null)
                referenceCulture = CultureInfo.CurrentCulture;
            else
                referenceCulture = culture;

            return referenceCulture.CompareInfo.IsSuffix(this.ToString(), value, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
        }

        [Pure]
        internal bool EndsWith(char value) {
            int thisLen = this.Length;
            if (thisLen != 0) {
                if (this[thisLen - 1] == value)
                    return true;
            }

            return false;
        }

        // Returns the index of the last occurance of any character in value in the current instance.
        // The search starts at startIndex and runs to endIndex. [startIndex,endIndex].
        // The character at position startIndex is included in the search.  startIndex is the larger
        // index within the string.
        //


        /// <summary>Creates a new object that is a copy of the current instance.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public abstract object Clone();
    }
}