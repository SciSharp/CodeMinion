using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Regen.DataTypes {
    /// <summary>
    ///     Base data type, all stored data types (e.g. <see cref="StringScalar"/>, <see cref="NullScalar"/>) inherit this.
    /// </summary>
    public abstract class Data : IEquatable<Data> {
        /// <summary>
        ///     A singleton of <see cref="NullScalar"/>, use this or new NullScalar()
        /// </summary>
        public static NullScalar Null { get; } = new NullScalar();

        public abstract object Value { get; set; }

        /// <summary>
        ///     Emit the <see cref="Value"/> for generation purposes.
        /// </summary>
        /// <returns></returns>
        public abstract string Emit();

        /// <summary>
        ///     Emit the <see cref="Value"/> for expression evaluation purposes.
        /// </summary>
        /// <returns></returns>
        public abstract string EmitExpressive();

        public static Data Create(object obj) {
            switch (obj) {
                case null:
                    return new NullScalar();
                case Data dd:
                    return dd;
                case string str:
                    return new StringScalar(str);
                case bool bl:
                    return new BoolScalar(bl);
                case IComparable _num:
                    var type = _num.GetType();
                    if (type.IsPrimitive || type == typeof(decimal))
                        return new NumberScalar(_num);
                    return new NetObject(_num);
                case List<Data> sc:
                    return new Array(sc);
                case IList<Data> sc:
                    return new Array(sc.ToList());
                case IEnumerable<Data> en:
                    return new Array(en.ToList());
                default:
                    return new NetObject(obj);
            }
        }

        #region Operators

        public static object operator +(Data sc) {
            dynamic lhs = sc.Value;
            return tryParseNumber(+lhs);
        }

        public static object operator !(Data sc) {
            dynamic lhs = sc.Value;
            return tryParseNumber(!lhs);
        }

        public static object operator -(Data sc) {
            dynamic rhs = sc.Value;
            return tryParseNumber(-rhs);
        }

        public static object operator +(Data sc, object v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v;
            return tryParseNumber(lhs + rhs);
        }

        public static object operator +(object v, Data sc) {
            dynamic lhs = v;
            dynamic rhs = sc.Value;
            return tryParseNumber(lhs + rhs);
        }

        public static object operator +(Data sc, Data v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v.Value;
            var ret = lhs + rhs;
            return tryParseNumber(ret);
        }

        public static object operator -(Data sc, object v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v;
            return tryParseNumber(lhs - rhs);
        }

        public static object operator -(object v, Data sc) {
            dynamic lhs = v;
            dynamic rhs = sc.Value;
            return tryParseNumber(lhs - rhs);
        }

        public static object operator -(Data sc, Data v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v.Value;
            return tryParseNumber(lhs - rhs);
        }

        public static object operator /(Data sc, object v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v;
            return tryParseNumber(lhs / rhs);
        }

        public static object operator /(object v, Data sc) {
            dynamic lhs = v;
            dynamic rhs = sc.Value;
            return tryParseNumber(lhs / rhs);
        }

        public static object operator /(Data sc, Data v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v.Value;
            return tryParseNumber(lhs / rhs);
        }

        public static object operator %(Data sc, object v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v;
            return tryParseNumber(lhs % rhs);
        }

        public static object operator %(object v, Data sc) {
            dynamic lhs = v;
            dynamic rhs = sc.Value;
            return tryParseNumber(lhs % rhs);
        }

        public static object operator %(Data sc, Data v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v.Value;
            return tryParseNumber(lhs % rhs);
        }

        public static object operator *(Data sc, object v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v;
            return tryParseNumber(lhs * rhs);
        }

        public static object operator *(object v, Data sc) {
            dynamic lhs = v;
            dynamic rhs = sc.Value;
            return tryParseNumber(lhs * rhs);
        }

        public static object operator *(Data sc, Data v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v.Value;
            return tryParseNumber(lhs * rhs);
        }

        public static object operator &(Data sc, object v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v;
            return tryParseNumber(lhs & rhs);
        }

        public static object operator &(object v, Data sc) {
            dynamic lhs = v;
            dynamic rhs = sc.Value;
            return tryParseNumber(lhs & rhs);
        }

        public static object operator &(Data sc, Data v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v.Value;
            return tryParseNumber(lhs & rhs);
        }

        public static object operator |(Data sc, object v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v;
            return tryParseNumber(lhs | rhs);
        }

        public static object operator |(object v, Data sc) {
            dynamic lhs = v;
            dynamic rhs = sc.Value;
            return tryParseNumber(lhs | rhs);
        }

        public static object operator |(Data sc, Data v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v.Value;
            return tryParseNumber(lhs | rhs);
        }

        
        private static object tryParseNumber(object obj) {
            Type type = obj.GetType();
            if (obj is bool b)
                return new BoolScalar(b);
            if (type.IsPrimitive || type == typeof(decimal))
                return new NumberScalar(obj);

            return obj;
        }
        #endregion

        #region Equality

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(Data other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Value, other.Value);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Data) obj);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() {
            return (Value != null ? Value.GetHashCode() : 0);
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Regen.DataTypes.Data" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(Data left, Data right) {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Regen.DataTypes.Data" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(Data left, Data right) {
            return !Equals(left, right);
        }

        #endregion
    }
}