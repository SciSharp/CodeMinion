using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Regen.Exceptions;
using Regen.Helpers;

namespace Regen.Parser.Expressions {
    public class ReferenceIdentity : Identity {
        protected static readonly string _literalRegex;
        protected RegexResult _match;

        static ReferenceIdentity() {
            _literalRegex = AttributeExtensions.GetAttribute<ExpressionTokenAttribute>(ExpressionToken.Literal).Regex;
        }

        public string Name { get; set; }

        protected ReferenceIdentity(string name) {
            Name = name;
        }

        public ReferenceIdentity(string name, RegexResult res) {
            Name = name;
            _match = res;
        }

        public static ReferenceIdentity Create(Match match) {
            var name = match.Value;
            if (!Regex.IsMatch(name, _literalRegex, Regexes.DefaultRegexOptions))
                throw new ExpressionException($"The name '{name}' contains invalid symbols. Regex Pattern: {_literalRegex}");
            return new ReferenceIdentity(name) {_match = match.AsResult()};
        }

        public static ReferenceIdentity Parse(ExpressionWalker ew) {
            //types:
            //justname
            ew.IsCurrentOrThrow(ExpressionToken.Literal);
            var ret = new ReferenceIdentity(ew.Current.Match.Value) {_match = ew.Current.Match.AsResult()};
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
        public bool Equals(ReferenceIdentity other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(string other) {
            return Equals(other, Name);
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(StringLiteral other) {
            return Equals(other?.Value, Name);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ReferenceIdentity) obj);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Regen.Parser.Expressions.StringIdentity" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(ReferenceIdentity left, ReferenceIdentity right) {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Regen.Parser.Expressions.StringIdentity" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(ReferenceIdentity left, ReferenceIdentity right) {
            return !Equals(left, right);
        }

        #endregion

        public static ReferenceIdentity Wrap(StringIdentity iden) {
            return new ReferenceIdentity(iden.Name, iden.Matches().First());
        }
    }
}