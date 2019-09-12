using System;
using Regen.DataTypes;
using Regen.Helpers;

namespace Regen.Builtins {
    public partial class CommonExpressionFunctions {
        public static BoolScalar op_equals(object left, object right) {
            left = unpack(left);
            right = unpack(right);

            if (Equals(left, right))
                return true;

            var a = left as Data;
            var b = right as Data;
            if (a != null && b != null)
                return Equals(a.Value, b.Value);

            return false;
        }

        public static BoolScalar op_notequals(object left, object right) {
            return !op_equals(left, right);
        }

        public static BoolScalar op_smaller(object left, object right) {
            left = unpack(left);
            right = unpack(right);

            if (left is IComparable lhs && right is IComparable rhs)
                return lhs.CompareTo(rhs) == -1;

            return false;
        }

        public static BoolScalar op_smallerequals(object left, object right) {
            left = unpack(left);
            right = unpack(right);

            if (left is IComparable lhs && right is IComparable rhs)
                return lhs.CompareTo(rhs) <= 0;

            return false;
        }

        public static BoolScalar op_bigger(object left, object right) {
            left = unpack(left);
            right = unpack(right);

            if (left is IComparable lhs && right is IComparable rhs)
                return lhs.CompareTo(rhs) == 1;

            return false;
        }

        public static BoolScalar op_biggerequals(object left, object right) {
            left = unpack(left);
            right = unpack(right);

            if (left is IComparable lhs && right is IComparable rhs)
                return lhs.CompareTo(rhs) >= 0;

            return false;
        }

        public static BoolScalar op_not(object operand) {
            operand = unpack(operand);
            if (operand is bool b)
                return !b;

            dynamic oper = operand;

            return !oper;
        }

        public static BoolScalar op_or(object left, object right) {
            left = unpack(left);
            right = unpack(right);
            dynamic lhs = left;
            dynamic rhs = right;

            return lhs | rhs;
        }

        public static BoolScalar op_inverse(object operand) {
            operand = unpack(operand);
            dynamic lhs = operand;

            return ~lhs;
        }

        public static BoolScalar op_and(object left, object right) {
            left = unpack(left);
            right = unpack(right);

            dynamic lhs = left;
            dynamic rhs = right;

            return lhs & rhs;
        }

        public static BoolScalar op_equalsapprox(object left, object right) {
            left = unpack(left);
            right = unpack(right);

            dynamic lhs = left;
            dynamic rhs = right;
            var sub = (lhs - rhs);
            return (sub < 0 ? -sub : sub) < 0.0001;
        }

        private static object unpack(object operand) {
            if (operand is ReferenceData lr)
                operand = lr.Value;

            if (operand is Data ld)
                operand = ld.Value;

            return operand;
        }
    }
}