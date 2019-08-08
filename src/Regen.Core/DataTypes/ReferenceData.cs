using System;
using Regen.Flee.PublicTypes;

namespace Regen.DataTypes
{
    /// <summary>
    ///     This represents a reference to an other variable. <see cref="Value"/> is most likely a string with the variable name.
    /// </summary>
    public class ReferenceData : Data, IVariableReference, IEquatable<ReferenceData>
    {
        public override object Value { get; set; }

        public ReferenceData(object value)
        {
            Value = value;
        }

        /// <summary>
        ///     Emit the <see cref="Data.Value"/> for generation purposes.
        /// </summary>
        /// <returns></returns>
        public override string Emit()
        {
            if (Value is Data d)
            {
                return d.Emit();
            }

            return Value.ToString();
        }

        /// <summary>
        ///     Emit the <see cref="Data.Value"/> for expression evaluation purposes.
        /// </summary>
        /// <returns></returns>
        public override string EmitExpressive()
        {
            if (Value is Data d)
            {
                return d.EmitExpressive();
            }

            return Value.ToString();
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Emit();
        }

        string IVariableReference.Target => (string) Value;

        #region Equality

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(ReferenceData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Value, other.Value);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return Equals(Value, obj);
            return Equals(Value, ((ReferenceData) obj).Value);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Regen.DataTypes.ReferenceData" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(ReferenceData left, ReferenceData right)
        {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Regen.DataTypes.ReferenceData" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(ReferenceData left, ReferenceData right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}