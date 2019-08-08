using System;
using System.Diagnostics;

namespace Regen.DataTypes {
    /// <summary>
    ///     A simple wrapper that is compatible with Regen's type system.
    /// </summary>
    [DebuggerDisplay("NetObject: {" + nameof(Value) + "}")]
    public class NetObject : Data, IEquatable<NetObject> {
        public NetObject(object obj) {
            Value = obj;
        }

        public override object Value { get; set; }

        /// <summary>
        ///     Emit the <see cref="Data.Value"/> for generation purposes.
        /// </summary>
        /// <returns></returns>
        public override string Emit() {
            return Value.ToString();
        }

        /// <summary>
        ///     Emit the <see cref="Data.Value"/> for expression evaluation purposes.
        /// </summary>
        /// <returns></returns>
        public override string EmitExpressive() {
            return Emit();
        }

        #region Operators

        

        #endregion

        #region Equality

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(NetObject other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(Value, other.Value);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NetObject) obj);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() {
            return base.GetHashCode();
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Regen.DataTypes.NetObject" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(NetObject left, NetObject right) {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Regen.DataTypes.NetObject" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(NetObject left, NetObject right) {
            return !Equals(left, right);
        }

        #endregion
    }
}