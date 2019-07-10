using System;
using System.Diagnostics;

namespace Regen.DataTypes {
    [DebuggerDisplay("Boolean: {" + nameof(Value) + "}")]
    public class BoolScalar : Data, IEquatable<BoolScalar> {
        public bool _value => (bool) Value;
        public override object Value { get; set; }

        public BoolScalar(bool value) {
            Value = value;
        }

        /// <summary>
        ///     Emit the <see cref="Data.Value"/> for generation purposes.
        /// </summary>
        /// <returns></returns>
        public override string Emit() {
            return _value.ToString();
        }

        /// <summary>
        ///     Emit the <see cref="Data.Value"/> for expression evaluation purposes.
        /// </summary>
        /// <returns></returns>
        public override string EmitExpressive() {
            return _value.ToString();
        }

        #region Equality

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(BoolScalar other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && _value == other._value;
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BoolScalar) obj);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() {
            unchecked {
                return (base.GetHashCode() * 397) ^ _value.GetHashCode();
            }
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Regen.DataTypes.BoolScalar" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(BoolScalar left, BoolScalar right) {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Regen.DataTypes.BoolScalar" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(BoolScalar left, BoolScalar right) {
            return !Equals(left, right);
        }

        #endregion
    }
}