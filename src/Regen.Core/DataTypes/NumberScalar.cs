using System;
using System.Diagnostics;
using System.Linq;

namespace Regen.DataTypes {
    [DebuggerDisplay("Number: {" + nameof(Value) + "}")]
    public class NumberScalar : Scalar, IEquatable<NumberScalar>, IComparable {

        public override string Emit() {
            return Value.ToString();
        }

        /// <summary>
        ///     Emit the <see cref="Data.Value"/> for expression evaluation purposes.
        /// </summary>
        /// <returns></returns>
        public override string EmitExpressive() {
            var emission = Value.ToString();

            if (!emission.Contains(".")) {
                switch (Value) {
                    case Double @double:
                        emission += ".0d";
                        break;
                    case Single single:
                        emission += ".0f";
                        break;
                    case Decimal @decimal:
                        emission += ".0M";
                        break;
                }
            } else if (!char.IsLetter(emission.Last())) {
                switch (Value) {
                    case Double @double:
                        emission += "d";
                        break;
                    case Single single:
                        emission += "f";
                        break;
                    case Decimal @decimal:
                        emission += "M";
                        break;
                }
            }

            return emission;
        }

        //todo add MaxValue and MinValue
        //todo unit test those three
        public NumberScalar MaxValue() {
            dynamic val = Value;
            return new NumberScalar(val.MaxValue);
        }

        public NumberScalar MinValue() {
            dynamic val = Value;
            return new NumberScalar(val.MinValue);
        }

        public NumberScalar Zero() {
            return new NumberScalar(Activator.CreateInstance(Value.GetType()));
        }

        public T As<T>(T @default) where T : IComparable {
            return Value is T ret ? ret : @default;
        }

        public T Cast<T>() where T : IComparable {
            return (T) Convert.ChangeType(Value, typeof(T));
        }

        public NumberScalar(object value) : base(value is NumberScalar s ? s.Value : value) { }

        #region Equality

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(NumberScalar other) {
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
            return Equals((NumberScalar) obj);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() {
            unchecked {
                return (base.GetHashCode() * 397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        /// <summary>Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.</summary>
        /// <param name="obj">An object to compare with this instance. </param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj" /> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows <paramref name="obj" /> in the sort order. </returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="obj" /> is not the same type as this instance. </exception>
        public int CompareTo(object obj) {
            return (Value as IComparable)?.CompareTo(obj) ?? -2;
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Regen.DataTypes.NumberScalar" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(NumberScalar left, NumberScalar right) {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Regen.DataTypes.NumberScalar" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(NumberScalar left, NumberScalar right) {
            return !Equals(left, right);
        }

        #endregion
    }
}