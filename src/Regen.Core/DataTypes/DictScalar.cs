using System;
using System.Collections.Generic;
using Regen.Helpers.Collections;

namespace Regen.DataTypes {
    public class Dictionary : Data, IEquatable<Dictionary>, IEquatable<Dict> {
        private Dict dict {
            get => (Dict) Value;
            set => Value = value;
        }

        #region Constructors

        public Dictionary(Dict dict) {
            this.dict = dict;
        }

        public Dictionary(int capacity) {
            dict = new Dict(capacity);
        }

        public Dictionary(IEqualityComparer<string> comparer) {
            dict = new Dict(comparer);
        }

        public Dictionary(int capacity, IEqualityComparer<string> comparer) {
            dict = new Dict(capacity, comparer);
        }

        public Dictionary(IDictionary<string, object> dictionary) {
            dict = new Dict(dictionary);
        }

        public Dictionary(IEnumerable<KeyValuePair<string, object>> keyValuePairs) {
            dict = new Dict(keyValuePairs);
        }

        public Dictionary(IDictionary<string, object> dictionary, IEqualityComparer<string> comparer) {
            dict = new Dict(dictionary, comparer);
        }

        public Dictionary(params object[] inlines) {
            dict = new Dict(inlines);
        }

        #endregion

        public override object Value { get; set; }

        /// <summary>
        ///     Emit the <see cref="Data.Value"/> for generation purposes.
        /// </summary>
        /// <returns></returns>
        public override string Emit() {
            throw new System.NotImplementedException();
        }

        /// <summary>
        ///     Emit the <see cref="Data.Value"/> for expression evaluation purposes.
        /// </summary>
        /// <returns></returns>
        public override string EmitExpressive() {
            throw new System.NotImplementedException();
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(Dictionary other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(dict, other.dict);
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(Dict other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(dict, other)) return true;
            return dict.Equals(other);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Dictionary) obj);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() {
            unchecked {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (dict != null ? dict.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Regen.DataTypes.Dictionary" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(Dictionary left, Dictionary right) {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Regen.DataTypes.Dictionary" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(Dictionary left, Dictionary right) {
            return !Equals(left, right);
        }
    }
}