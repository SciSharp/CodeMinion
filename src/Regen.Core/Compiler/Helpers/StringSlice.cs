using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Regen.Compiler.Helpers {
    public class StringSlice : StringSpan, IDisposable, IEnumerable {
        private StringSource _spanner;
        private Range _range;
        public StringSlice(int start, int end, StringSource spanner) : this(new Range(start, end), spanner) { }

        public StringSlice(Range range, StringSource spanner) {
            Range = range;
            _spanner = spanner;
        }

        /// <summary>
        ///     Was this slice deleted?
        /// </summary>
        public bool Deleted => End <= -1;

        public override int Start => Range.Start;
        public override int End => Range.End;

        /// <summary>
        ///     The range this slice represents
        /// </summary>
        public Range Range {
            get => _range;
            internal set => _range = value.End < value.Start ? Range.Empty : value;
        }

        /// <summary>
        ///     The length of this slice.
        /// </summary>
        public override int Length => End - Start + 1;

        /// <summary>
        ///     Duplicates current slice and inserts it into the next index after the end of this slice.
        /// </summary>
        /// <param name="expand">
        ///     If true: the current slice will expand and self is return.<br></br>
        ///     if false: current slice does not change and a new slice is returned.
        /// </param>
        public StringSlice Duplicate(bool expand) {
            _spanner.Insert(End + 1, ToString());
            if (expand) {
                Range = new Range(Range.Start, Range.End + Length);
                return this;
            }

            return new StringSlice(End + 1, End + Length, _spanner);
        }

        public override int Remove(int start, int length) {
            return Remove(Range.FromStartLength(start, length));
        }

        public override bool RemoveAt(int index) {
            var at = this.Start + index;
            return _spanner.RemoveAt(at);
        }

        public override int Remove(Range range) {
            return _spanner.Remove(new Range(this.Start + range.Start, this.Start + range.End));
        }

        /// <summary>
        ///     The characters this string holds.
        /// </summary>
        public override List<char> Chars {
            get => this.ToCharArray().ToList();
            protected set => ReplaceWith(value);
        }

        public override void Add(string str) {
            this.Add(str, true);
        }

        public override void Add(IEnumerable<char> chars) {
            this.Add(chars, true);
        }

        public override StringSlice Substring(int index, int length) {
            return Substring(Range.FromStartLength(index, length));
        }

        public override StringSlice Substring(Range range) {
            return _spanner.Substring(new Range(Start + range.Start, Start + range.End));
        }

        public override int Remove(int index) {
            return _spanner.Remove(new Range(Start + index, Start + (Length - index)));
        }

        public override void Insert(int index, string str) {
            _spanner.Insert(Start + index, str);
        }

        public override void Insert(int index, params char[] chars) {
            _spanner.Insert(Start + index, chars);
        }

        public override void ExchangeAt(int index, int endindex, string place) {
            _spanner.ExchangeAt(Start + index, Start + endindex, place);
        }

        public override void ExchangeAt(int index, int endindex, IEnumerable<char> chars) {
            _spanner.ExchangeAt(Start, End, chars);
        }

        /// <summary>
        ///     Sequence of removing and then placing without making slices to turn deleted.
        /// </summary>
        public override void ExchangeAt(int index, string place) {
            ExchangeAt(index, place.Length + index, place);
        }

        public override void ReplaceWith(string place) {
            _spanner.ExchangeAt(Start, End, place);
        }

        public override void ReplaceWith(IEnumerable<char> chars) {
            _spanner.ExchangeAt(Start, End, chars);
        }

        /// <summary>
        ///     Tests if given <see cref="index"/> is a valid index to access this <see cref="StringSpan"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override bool IsIndexInside(int index) {
            return index >= 0 && index <= End - Start;
        }

        public override char this[int index] {
            get => _spanner.Chars[Start + index];
            set => _spanner.Chars[Start + index] = value;
        }

        public void Add(string str, bool expand) {
            _spanner.Insert(End + 1, str);
            if (expand)
                Range = new Range(Range.Start, End + str.Length);
        }

        public void Add(IEnumerable<char> chars, bool expand) {
            var arr = (chars as char[] ?? chars).ToArray();
            _spanner.Insert(Start + 1, arr);
            if (expand)
                Range = new Range(Range.Start, End + arr.Length);
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


        public override bool Contains(IList<char> niddle) {
            int niddleLength = niddle.Count;
            int length = Length - niddleLength;
            for (int i = 0; i < length; i++) {
                for (int j = 0; j < niddleLength; j++) {
                    if (this[i + j] != niddle[j])
                        goto _next;
                }

                return true;
                _next:
                continue;
            }

            return false;
        }

        public override bool Contains(string niddle) {
            int niddleLength = niddle.Length;
            int length = Length - niddleLength;
            for (int i = 0; i < length; i++) {
                for (int j = 0; j < niddleLength; j++) {
                    if (this[i + j] != niddle[j])
                        goto _next;
                }

                return true;
                _next:
                continue;
            }

            return false;
        }

        public override bool Contains(StringSpan niddle) {
            int length = Length;
            int niddleLength = niddle.Length;
            for (int i = 0; i < length; i++) {
                for (int j = 0; j < niddleLength; j++) {
                    if (this[i + j] != niddle[j])
                        goto _next;
                }

                return true;
                _next:
                continue;
            }

            return false;
        }

        public override char[] ToCharArray() {
            var arr = new char[End - Start + 1]; //1 based
            _spanner.Chars.CopyTo(Start, arr, 0, arr.Length);
            return arr;
        }

        public override StringSpan[] Split(string separator, StringSplitOptions options) {
            var value = this.ToString();
            var @return = new List<StringSpan>();
            var indexes = Regex.Matches(value, Regex.Escape(separator), RegexOptions.CultureInvariant).Cast<Match>();
            int startIndex = 0;

            foreach (var match in indexes) {
                if (match.Index != 0) {
                    var to = match.Index - 1;
                    @return.Add(Substring(new Range(startIndex, to)));
                }

                startIndex = match.Index + match.Length;
            }

            if (startIndex < value.Length)
                @return.Add(Substring(new Range(startIndex, value.Length - 1)));
            if (options.HasFlag(StringSplitOptions.RemoveEmptyEntries)) {
                @return.RemoveAll(sp => sp.Length == 0);
            }

            return @return.ToArray();
        }

        public override StringSpan[] Split(char @char, StringSplitOptions options) {
            return Split(@char.ToString(), options);
        }

        /// <summary>
        ///     Add more characters to the end of this string
        /// </summary>
        /// <param name="chars">Number of characters (1 based) to add</param>
        /// <param name="fill">If it is the end of the string, <paramref name="fill"/> will be used as value.</param>
        public override void Extend(int chars, char fill = '\0') {
            var end = Range.End + chars;
            while (!_spanner.IsIndexInside(end))
                end--;
            Range = new Range(Range.Start, end);
        }

        /// <summary>Creates a new object that is a copy of the current instance.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone() {
            return _spanner.Substring(new Range(Start, End));
        }


        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<char> GetEnumerator() {
            return ToString().GetEnumerator();
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() {
            if (End == -1 || Start == -1)
                return string.Empty;
            var arr = new char[End - Start + 1]; //1 based
            _spanner.Chars.CopyTo(Start, arr, 0, arr.Length);
            return new string(arr);
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() {
            _spanner.Slices.Remove(this);
            _spanner = null;
            Range = Range.Empty;
        }
    }
}