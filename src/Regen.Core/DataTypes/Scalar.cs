using System;

namespace Regen.DataTypes {
    public abstract class Scalar : Data {
        public override object Value { get; set; }

        protected Scalar(object value) {
            Value = value;
        }

        public static Scalar Create(object value) {
            switch (value) {
                case null:
                    return new NullScalar();
                case Scalar sc:
                    return sc;
                case string str:
                    return new StringScalar(str);
                case IComparable _num:
                    var type = _num.GetType();
                    if (type.IsPrimitive || type == typeof(decimal))
                        return new NumberScalar(_num);
                    throw new TypeLoadException();
                default:
                    throw new TypeLoadException();
            }
        }

        #region Operators

        public static object operator +(Scalar sc) {
            dynamic lhs = sc.Value;
            return +lhs;
        }

        public static object operator !(Scalar sc) {
            dynamic lhs = sc.Value;
            return !lhs;
        }

        public static object operator -(Scalar sc) {
            dynamic rhs = sc.Value;
            return -rhs;
        }

        public static object operator +(Scalar sc, object v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v;
            return lhs + rhs;
        }

        public static object operator +(object v, Scalar sc) {
            dynamic lhs = v;
            dynamic rhs = sc.Value;
            return lhs + rhs;
        }

        public static object operator +(Scalar sc, Scalar v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v.Value;
            return lhs + rhs;
        }

        public static object operator -(Scalar sc, object v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v;
            return lhs - rhs;
        }

        public static object operator -(object v, Scalar sc) {
            dynamic lhs = v;
            dynamic rhs = sc.Value;
            return lhs - rhs;
        }

        public static object operator -(Scalar sc, Scalar v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v.Value;
            return lhs - rhs;
        }

        public static object operator /(Scalar sc, object v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v;
            return lhs / rhs;
        }

        public static object operator /(object v, Scalar sc) {
            dynamic lhs = v;
            dynamic rhs = sc.Value;
            return lhs / rhs;
        }

        public static object operator /(Scalar sc, Scalar v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v.Value;
            return lhs / rhs;
        }

        public static object operator %(Scalar sc, object v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v;
            return lhs % rhs;
        }

        public static object operator %(object v, Scalar sc) {
            dynamic lhs = v;
            dynamic rhs = sc.Value;
            return lhs % rhs;
        }

        public static object operator %(Scalar sc, Scalar v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v.Value;
            return lhs % rhs;
        }

        public static object operator *(Scalar sc, object v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v;
            return lhs * rhs;
        }

        public static object operator *(object v, Scalar sc) {
            dynamic lhs = v;
            dynamic rhs = sc.Value;
            return lhs * rhs;
        }

        public static object operator *(Scalar sc, Scalar v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v.Value;
            return lhs * rhs;
        }

        public static object operator &(Scalar sc, object v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v;
            return lhs & rhs;
        }

        public static object operator &(object v, Scalar sc) {
            dynamic lhs = v;
            dynamic rhs = sc.Value;
            return lhs & rhs;
        }

        public static object operator &(Scalar sc, Scalar v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v.Value;
            return lhs & rhs;
        }

        public static object operator |(Scalar sc, object v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v;
            return lhs | rhs;
        }

        public static object operator |(object v, Scalar sc) {
            dynamic lhs = v;
            dynamic rhs = sc.Value;
            return lhs | rhs;
        }

        public static object operator |(Scalar sc, Scalar v) {
            dynamic lhs = sc.Value;
            dynamic rhs = v.Value;
            return lhs | rhs;
        }


        #region Null

        public static object operator +(Scalar sc, NullScalar v) {
            return sc.Value;
        }

        public static object operator +(NullScalar v, Scalar sc) {
            return sc.Value;
        }

        public static object operator -(Scalar sc, NullScalar v) {
            return sc.Value;
        }

        public static object operator -(NullScalar v, Scalar sc) {
            return -sc;
        }


        public static object operator /(Scalar sc, NullScalar v) {
            return 0;
        }

        public static object operator /(NullScalar v, Scalar sc) {
            return 0;
        }


        public static object operator %(Scalar sc, NullScalar v) {
            return 0;
        }

        public static object operator %(NullScalar v, Scalar sc) {
            return 0;
        }


        public static object operator *(Scalar sc, NullScalar v) {
            return 0;
        }

        public static object operator *(NullScalar v, Scalar sc) {
            return 0;
        }


        public static object operator &(Scalar sc, NullScalar v) {
            dynamic lhs = sc.Value;
            return lhs & 0;
        }

        public static object operator &(NullScalar v, Scalar sc) {
            dynamic rhs = sc.Value;
            return 0 & rhs;
        }

        public static object operator |(Scalar sc, NullScalar v) {
            dynamic lhs = sc.Value;
            return lhs | 0;
        }

        public static object operator |(NullScalar v, Scalar sc) {
            dynamic rhs = sc.Value;
            return 0 | rhs;
        }

        #endregion

        #endregion
    }
}