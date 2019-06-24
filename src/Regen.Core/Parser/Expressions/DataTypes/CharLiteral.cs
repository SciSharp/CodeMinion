using System;
using System.Collections.Generic;
using Regen.Helpers;

namespace Regen.Parser.Expressions {
    public class CharLiteral : Expression, IEquatable<CharLiteral>, IEquatable<char> {
        private RegexResult _match;
        public char Value;

        public CharLiteral(char v) {
            Value = v;
        }

        public static CharLiteral Parse(ExpressionWalker ew) {
            ew.IsCurrentOrThrow(ExpressionToken.CharLiteral);
            var ret = new CharLiteral(char.Parse(ew.Current.Match.Groups[1].Value)) {_match = ew.Current.Match.AsResult()};
            ew.Next();
            return ret;
        }

        public override IEnumerable<RegexResult> Matches() {
            yield return _match;
        }

        #region Equality

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(CharLiteral other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Value, other.Value);
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(char other) {
            return Equals(Value, other);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CharLiteral) obj);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() {
            return (Value.GetHashCode());
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Regen.Parser.Expressions.StringLiteral" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(CharLiteral left, CharLiteral right) {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Regen.Parser.Expressions.StringLiteral" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(CharLiteral left, CharLiteral right) {
            return !Equals(left, right);
        }

        #endregion
    }
}