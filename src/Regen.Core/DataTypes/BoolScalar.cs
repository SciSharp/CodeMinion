using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Regen.DataTypes
{
    [DebuggerDisplay("Boolean: {" + nameof(Value) + "}")]
    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
    public class BoolScalar : Data, IEquatable<BoolScalar>
    {
        public bool _value => (bool) Value;
        public override object Value { get; set; }

        public BoolScalar(bool value)
        {
            Value = value;
        }

        /// <summary>
        ///     Emit the <see cref="Data.Value"/> for generation purposes.
        /// </summary>
        /// <returns></returns>
        public override string Emit()
        {
            return _value.ToString().ToLower(CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Emit the <see cref="Data.Value"/> for expression evaluation purposes.
        /// </summary>
        /// <returns></returns>
        public override string EmitExpressive()
        {
            return _value.ToString();
        }

        #region Equality

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(BoolScalar other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && _value == other._value;
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(bool other)
        {
            return _value == other;
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is bool b)
                return Equals(b);
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BoolScalar) obj);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Regen.DataTypes.BoolScalar" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(BoolScalar left, BoolScalar right)
        {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Regen.DataTypes.BoolScalar" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(BoolScalar left, BoolScalar right)
        {
            return !Equals(left, right);
        }

        public static implicit operator BoolScalar(bool boolean) => new BoolScalar(boolean);

        public static implicit operator bool(BoolScalar boolean) => boolean._value;


        public static BoolScalar operator !(BoolScalar b) => !b._value;

        /// <summary>Returns a value that indicates whether two <see cref="T:Regen.DataTypes.BoolScalar" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator &(BoolScalar left, BoolScalar right) => Equals(left, true) && Equals(right, true);

        /// <summary>Returns a value that indicates whether two <see cref="T:Regen.DataTypes.BoolScalar" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator |(BoolScalar left, BoolScalar right) => Equals(left, true) || Equals(right, true);

        #endregion
    }
}