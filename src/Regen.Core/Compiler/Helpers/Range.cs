using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Regen.Compiler.Expressions;

namespace Regen.Compiler.Helpers {
    [DebuggerDisplay("Range({Start} -> {End})")]
    public readonly struct Range : IEquatable<Range> {
        public static Range Empty { get; } = new Range(-1, -1);
        public readonly int Start;
        public readonly int End;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        [DebuggerStepThrough]
        public Range(int start, int end) {
            Start = start;
            End = end;
        }

        public (int Index, int Length) AsStartLength() => (Start, End - Start + 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Range FromStartLength(int start, int length) {
            return new Range(start, start + length - 1);
        }

        public Range GetIntersection(Range other) {
            if (Contains(other))
                return other;
            if (!Intersect(other))
                return default;

            return new Range(Math.Max(Start, other.Start), Math.Min(End, other.End));
        }

        //0123456
        //  234
        //  012
        public static Range Subtract(Range left, Range right) {
            if (left < right) {
                //#1
                return left;
            }

            if (right.Start == left.End && !left.ContainsIndex(right.End)) {
                //#4
                return new Range(left.Start, left.End - 1);
            }

            if (right.End == left.Start && !left.ContainsIndex(right.Start)) {
                //#5
                return new Range(left.Start - 1, left.End - 1);
            }

            if (right.End > left.End && left.ContainsIndex(right.Start)) {
                //#2
                return new Range(left.Start, left.End - (left.End - right.Start + 1));
            }

            if (right.Contains(left)) {
                //#3
                return Empty; //deleted bruh
            }

            if (left.Contains(right)) {
                //%6 //also checks if boundries are equal
                return new Range(left.Start, left.End - (right.End - right.Start + 1));
            }

            if (left > right) {
                //#7
                var len = right.End - right.Start + 1; //+1?
                return new Range(left.Start - len, left.End - len);
            }

            if (right.Start < left.Start && left.ContainsIndex(right.End)) {
                //#8
                var enddiff = right.End - left.Start + 1;
                var shareddiff = right.End - right.Start + 1;
                return new Range(left.Start - shareddiff, left.End - enddiff - shareddiff);
            }

            throw new NotImplementedException();
        }

        public static Range Insert(Range left, Range right) {
            if (left < right) {
                //#1
                return left;
            }

            if (right.Start < left.Start) {
                //#7,8,5,3
                var len = right.End - right.Start + 1; //+1?
                return new Range(left.Start + len, left.End + len);
            }

            if (left.Contains(right)) {
                //%6 //also checks if boundries are equal
                //redundant
            }

            if (left.ContainsIndex(right.Start)) {
                //covers #2,4,6
                return new Range(left.Start, left.End + (right.End - right.Start + 1));
            }

            throw new NotImplementedException();
        }

        /// <summary>
        ///     Adds <see cref="added"/> to right and adjusts left accodingly
        /// </summary>
        /// <param name="left">the target to adjust</param>
        /// <param name="right">the range that was before the add</param>
        /// <param name="added">how much the target got added (1 based) </param>
        /// <returns></returns>
        /// <remarks>The numbers cover the following image of possibilities</remarks>
        public static Range Add(Range left, Range right, int added) {
            if (right.End > left.End) {
                //#1,2,3,4
                return left;
            }

            if (left.ContainsIndex(right.End)) {
                //#5,6,8
                return new Range(left.Start, left.End + added);
            }

            if (right < left) {
                //#7
                return new Range(left.Start + added, left.End + added);
            }

            throw new NotImplementedException();
        }

        public static Range operator ++(Range range) {
            return new Range(range.Start + 1, range.End + 1);
        }

        public static Range operator --(Range range) {
            return new Range(range.Start - 1, range.End - 1);
        }

        public static Range operator -(Range range) {
            return new Range(-range.Start, -range.End);
        }

        /// <summary>
        ///     Does this range intersects with other range, partially or fully (inside it)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersect(Range other) {
            return this.ContainsIndex(other.Start) || this.ContainsIndex(other.End);
        }

        /// <summary>
        ///     Does this range contains <see cref="other"/> inside it?
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Range other) {
            return this.ContainsIndex(other.Start) && this.ContainsIndex(other.End);
        }

        /// <summary>
        ///     Is <see cref="index"/> contained inside this range.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <remarks>
        ///     This: Start=0, End=10<br></br>
        ///     index: 0    5   9   10  15<br></br>
        ///     return t    t   t   t   f
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsIndex(int index) {
            return index >= Start && index <= End;
        }

        /// <summary>
        ///     If range starts at 3 and ends at 6, this will return 3,4,5,6.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> EnumerateIndexes() {
            for (int i = Start; i <= End; i++) {
                yield return i;
            }
        }

        #region Equality

        public static bool operator >(Range left, Range right) {
            return left.End > right.End && left.Start > right.Start;
        }

        public static bool operator <(Range left, Range right) {
            return left.End < right.End && left.Start < right.Start;
        }

        public static bool operator <=(Range left, Range right) {
            return left.End <= right.End && left.Start <= right.Start;
        }

        public static bool operator >=(Range left, Range right) {
            return left.End >= right.End && left.Start >= right.Start;
        }

        public static bool operator ==(Range left, Range right) {
            return left.End == right.End && left.Start == right.Start;
        }

        public static bool operator !=(Range left, Range right) {
            return left.End != right.End && left.Start != right.Start;
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(Range other) {
            return Start == other.Start && End == other.End;
        }

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, <see langword="false" />. </returns>
        public override bool Equals(object obj) {
            return obj is Range other && Equals(other);
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode() {
            unchecked {
                return (Start * 397) ^ End;
            }
        }

        #endregion
    }
}