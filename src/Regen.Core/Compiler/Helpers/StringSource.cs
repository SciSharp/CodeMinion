using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Regen.Helpers.Collections;

namespace Regen.Compiler.Helpers {
    /// <summary>
    ///     A string can can be sliced (a Span{char} like System.Memory's) but the slices are tracked - meaning if the string changes - so do they.
    /// </summary>
    public class StringSource : StringSpan {
        public StringSource(IEnumerable<char> chars) {
            Chars = (chars as List<char> ?? chars).ToList();
        }

        public StringSource(string str) : this(str.ToCharArray().ToList()) { }
        public StringSource() : this(new List<char>(0)) { }

        /// <summary>
        ///     The tracked slices of this <see cref="StringSource"/>.
        /// </summary>
        public OList<StringSlice> Slices { get; set; } = new OList<StringSlice>();

        public override int Length => Chars.Count;

        public override char this[int index] {
            get => Chars[index];
            set => Chars[index] = value;
        }

        /// <summary>
        ///     The index the string span begins (inclusive)
        /// </summary>
        public override int Start => 0;

        /// <summary>
        ///     The index the string span ends (inclusive)
        /// </summary>
        public override int End => Chars.Count - 1;

        /// <summary>
        ///     The characters this string holds.
        /// </summary>
        public sealed override List<char> Chars { get; protected set; }


        public override void Add(string str) {
            var currentRange = new Range(0, Chars.Count - 1);
            var addedRange = new Range(Chars.Count, Chars.Count + str.Length - 1);
            Chars.AddRange(str.ToCharArray());
            for (var i = 0; i < Slices.Count; i++) {
                var slice = Slices[i];
                if (slice.Deleted)
                    continue;
                slice.Range = Range.Add(slice.Range, currentRange, str.Length);
            }
        }

        public override void Add(IEnumerable<char> chars) {
            var currentRange = new Range(0, Chars.Count - 1);
            var arr = (chars as char[] ?? chars).ToArray();
            Chars.AddRange(arr);
            for (var i = 0; i < Slices.Count; i++) {
                var slice = Slices[i];
                if (slice.Deleted)
                    continue;
                slice.Range = Range.Add(slice.Range, currentRange, arr.Length);
            }
        }

        public override StringSlice Substring(int index, int length) {
            return Substring(Range.FromStartLength(index, length));
        }

        public override StringSlice Substring(Range range) {
            if (range.Start < 0 || range.Start >= Chars.Count)
                throw new ArgumentOutOfRangeException(nameof(range.Start));
            if (range.End < 0 || range.End >= Chars.Count) //zero means empty string..
                throw new ArgumentOutOfRangeException(nameof(range.End));
            var slice = new StringSlice(range, this);
            Slices.Add(slice);
            return slice;
        }

        public override int Remove(int index) {
            return Remove(index, Chars.Count - index);
        }

        public override bool RemoveAt(int index) {
            return Remove(index, 1) == 1;
        }

        public override int Remove(int index, int length) {
            return Remove(Range.FromStartLength(index, length));
        }

        public override int Remove(Range range) {
            var length = range.End - range.Start + 1;
            Chars.RemoveRange(range.Start, length);
            for (var i = 0; i < Slices.Count; i++) {
                var slice = Slices[i];
                if (slice.Deleted)
                    continue;
                slice.Range = Range.Subtract(slice.Range, range);
            }

            return length;
        }

        public override void Insert(int index, params char[] chars) {
            Chars.InsertRange(index, chars);
            var range = new Range(index, index + chars.Length - 1);
            for (var i = 0; i < Slices.Count; i++) {
                var slice = Slices[i];
                if (slice.Deleted)
                    continue;
                slice.Range = Range.Insert(slice.Range, range);
            }
        }

        public override void Insert(int index, string str) {
            Insert(index, str.ToCharArray());
        }

        //public void Insert(int index, ReadOnlySpan<char> chars) {
        //    Insert(index, chars.ToArray());
        //}

        /// <summary>
        ///     Sequence of removing and then placing without making slices to turn deleted.
        /// </summary>
        public override void ExchangeAt(int index, int endindex, string place) {
            //3456
            //hello
            var addon = place.Length - (endindex - index + 1);
            var right = new Range(index, endindex);
            Chars.RemoveRange(index, endindex - index + 1);
            Chars.InsertRange(index, place.ToCharArray());

            //todo fix this mess.
            if (addon != 0)
                for (var i = 0; i < Slices.Count; i++) {
                    var slice = Slices[i];
                    if (slice.Deleted)
                        continue;
                    if (addon > 0)
                        slice.Range = Range.Add(slice.Range, right, addon);
                    else
                        slice.Range = Range.Subtract(slice.Range, new Range(endindex - (Math.Abs(addon) - 1), endindex)); //addon is negative
                    //todo it is possible that when addon is negative, this might fail.
                }
        }

        /// <summary>
        ///     Sequence of removing and then placing without making slices to turn deleted.
        /// </summary>
        public override void ExchangeAt(int index, string place) {
            ExchangeAt(index, index + place.Length, place);
        }

        /// <summary>
        ///     Sequence of removing and then placing without making slices to turn deleted.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="endindex"></param>
        /// <param name="chars"></param>
        public override void ExchangeAt(int index, int endindex, IEnumerable<char> chars) {
            ExchangeAt(index, endindex, new string(chars as char[] ?? chars.ToArray()));
        }

        public override int TrimEnd(params char[] chars) {
            int items = 0;
            for (var i = Length - 1; i >= 0; i--) {
                var @this = this[i];
                if (chars.All(oc => oc != @this))
                    break;
                items++;
            }

            if (items != 0)
                Remove(Length - items, items);
            return items;
        }

        public override int TrimStart(params char[] chars) {
            int items = 0;
            for (int i = 0; i < Length; i++) {
                var @this = this[i];
                if (chars.All(oc => oc != @this))
                    break;
                items++;
            }

            if (items != 0)
                Remove(0, items);
            return items;
        }

        public override int Trim(params char[] chars) {
            int total = 0;
            total += TrimEnd(chars);
            total += TrimStart(chars);
            return total;
        }

        public override char[] ToCharArray() {
            return Chars.ToArray();
        }

        public override StringSpan[] Split(string separator, StringSplitOptions options) {
            var value = this.ToString();
            var @return = new List<StringSpan>();
            var indexes = Regex.Matches(value, Regex.Escape(separator), RegexOptions.CultureInvariant).Cast<Match>();
            int startIndex = 0;

            foreach (var match in indexes) {
                var to = match.Index - 1;
                if (to == -1)
                    continue;
                var sub = Substring(new Range(startIndex, to));
                @return.Add(sub);

                startIndex = match.Index + match.Length;
            }

            if (startIndex < value.Length)
                @return.Add(Substring(new Range(startIndex, value.Length - 1)));
            if (options.HasFlag(StringSplitOptions.RemoveEmptyEntries)) {
                @return.RemoveAll(sp => sp.Length == 0 || sp.End == -1 || sp.Start == -1);
            }

            return @return.ToArray();
        }


        public override StringSpan[] Split(char @char, StringSplitOptions options) {
            return Split(@char.ToString(), options);
        }

        public override void ReplaceWith(string place) {
            ExchangeAt(0, Length - 1, place);
        }

        public override void ReplaceWith(IEnumerable<char> chars) {
            ExchangeAt(0, Length - 1, chars);
        }

        /// <summary>
        ///     Tests if given <see cref="index"/> is a valid index to access this <see cref="StringSpan"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override bool IsIndexInside(int index) {
            return index >= 0 && index <= Chars.Count - 1;
        }

        /// <summary>
        ///     Add more characters to the end of this string
        /// </summary>
        /// <param name="chars">Number of characters (1 based) to add</param>
        /// <param name="fill">If it is the end of the string, <paramref name="fill"/> will be used as value.</param>
        public override void Extend(int chars, char fill = '\0') {
            Add(new string(fill, chars));
        }


        public override bool Contains(IList<char> niddle) {
            if (niddle.Count > this.Length)
                return false;
            int length = Chars.Count;
            int niddleLength = niddle.Count;
            for (int i = 0; i < length; i++) {
                for (int j = 0; j < niddleLength; j++) {
                    if (Chars[i + j] != niddle[j])
                        goto _next;
                }

                return true;
                _next:
                continue;
            }

            return false;
        }

        public override bool Contains(string niddle) {
            if (niddle.Length > this.Length)
                return false;
            int niddleLength = niddle.Length;
            int length = Chars.Count - niddleLength;
            for (int i = 0; i < length; i++) {
                for (int j = 0; j < niddleLength; j++) {
                    if (Chars[i + j] != niddle[j])
                        goto _next;
                }

                return true;
                _next:
                continue;
            }

            return false;
        }

        public override bool Contains(StringSpan niddle) {
            if (niddle.Length > this.Length)
                return false;

            int niddleLength = niddle.Length;
            int length = Chars.Count - niddleLength;
            for (int i = 0; i < length; i++) {
                for (int j = 0; j < niddleLength; j++) {
                    if (Chars[i + j] != niddle[j])
                        goto _next;
                }

                return true;
                _next:
                continue;
            }

            return false;
        }

        public override object Clone() {
            return new StringSource(Chars.ToList());
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() {
            return new string(Chars.ToArray());
        }
    }
}