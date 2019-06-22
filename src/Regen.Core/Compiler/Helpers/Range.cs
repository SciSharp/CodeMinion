using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Regen.Compiler.Helpers {

    [DebuggerDisplay("Range({Start} -> {End})")]
    public readonly struct Range {
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

        public Range GetInstersection(Range other) {
            if (Contains(other))
                return other;
            if (!Intersect(other))
                return default;

            return new Range(Math.Max(Start, other.Start), Math.Min(End, other.End));
        }

        public static Range operator -(Range left, Range right) {
            if (left.Contains(right)) {
                return new Range(left.Start, left.End - (right.End - right.Start + 1));
            }

            if (left.Start > right.Start && left.ContainsIndex(right.End)) {
                var start = left.Start - (left.Start - right.Start);
                var end = left.End - (right.End - right.Start);
                return new Range(start, end);
            }


            if (right.End > left.End && left.ContainsIndex(right.Start)) {
                return new Range(left.Start, left.End - (left.End - right.Start + 1)); //
            }

            if (left >= right) {
                if (left.Start == right.End) {
                    return new Range(left.Start + 1, left.End);
                }

                var sdiff = right.End - right.Start + 1;
                return new Range(left.Start - sdiff, left.End - sdiff); //
            }

            if (right > left)
                return left;

            if (right >= left) {
                if (left.End == right.Start) {
                    return new Range(left.Start, left.End - 1);
                }

                //todo rewrite this... it doesnt work
                return left; //todo test this cunt
            }

            if (right.Contains(left)) {
                return Range.Empty;
            }

            throw new NotImplementedException();
        }

        public static Range operator +(Range left, Range right) {
            if (left.Contains(right)) {
                return new Range(left.Start, left.End + (right.End - right.Start));
            }

            if (left.Start > right.Start && left.ContainsIndex(right.End)) {
                var start = left.Start + (left.Start - right.Start);
                var end = left.End + (right.End - right.Start);
                return new Range(start, end);
            }


            if (right.End > left.End && left.ContainsIndex(right.Start)) {
                return new Range(left.Start, left.End + (left.End - right.Start));
            }

            if (left >= right) {
                var sdiff = right.End - right.Start;
                return new Range(left.Start + sdiff, left.End + sdiff);
            }

            if (right > left)
                return left;

            if (right >= left)
                return new Range(left.Start, left.End + (left.End - right.Start)); //todo test this cunt

            if (right.Contains(left)) {
                return Range.Empty;
            }

            throw new NotImplementedException();
        }

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
    }
}