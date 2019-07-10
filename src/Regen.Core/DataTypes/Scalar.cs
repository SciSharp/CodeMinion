using System;
using System.Collections;
using System.Collections.Generic;

namespace Regen.DataTypes {
    public abstract class Scalar : Data {
        public override object Value { get; set; }

        protected Scalar(object value) {
            Value = value;
        }

        //#region Operators

        //public static object operator +(Scalar sc) {
        //    dynamic lhs = sc.Value;
        //    return +lhs;
        //}

        //public static object operator !(Scalar sc) {
        //    dynamic lhs = sc.Value;
        //    return !lhs;
        //}

        //public static object operator -(Scalar sc) {
        //    dynamic rhs = sc.Value;
        //    return -rhs;
        //}

        //public static object operator +(Scalar sc, object v) {
        //    dynamic lhs = sc.Value;
        //    dynamic rhs = v;
        //    return lhs + rhs;
        //}

        //public static object operator +(object v, Scalar sc) {
        //    dynamic lhs = v;
        //    dynamic rhs = sc.Value;
        //    return lhs + rhs;
        //}

        //public static object operator +(Scalar sc, Scalar v) {
        //    dynamic lhs = sc.Value;
        //    dynamic rhs = v.Value;
        //    return lhs + rhs;
        //}

        //public static object operator -(Scalar sc, object v) {
        //    dynamic lhs = sc.Value;
        //    dynamic rhs = v;
        //    return lhs - rhs;
        //}

        //public static object operator -(object v, Scalar sc) {
        //    dynamic lhs = v;
        //    dynamic rhs = sc.Value;
        //    return lhs - rhs;
        //}

        //public static object operator -(Scalar sc, Scalar v) {
        //    dynamic lhs = sc.Value;
        //    dynamic rhs = v.Value;
        //    return lhs - rhs;
        //}

        //public static object operator /(Scalar sc, object v) {
        //    dynamic lhs = sc.Value;
        //    dynamic rhs = v;
        //    return lhs / rhs;
        //}

        //public static object operator /(object v, Scalar sc) {
        //    dynamic lhs = v;
        //    dynamic rhs = sc.Value;
        //    return lhs / rhs;
        //}

        //public static object operator /(Scalar sc, Scalar v) {
        //    dynamic lhs = sc.Value;
        //    dynamic rhs = v.Value;
        //    return lhs / rhs;
        //}

        //public static object operator %(Scalar sc, object v) {
        //    dynamic lhs = sc.Value;
        //    dynamic rhs = v;
        //    return lhs % rhs;
        //}

        //public static object operator %(object v, Scalar sc) {
        //    dynamic lhs = v;
        //    dynamic rhs = sc.Value;
        //    return lhs % rhs;
        //}

        //public static object operator %(Scalar sc, Scalar v) {
        //    dynamic lhs = sc.Value;
        //    dynamic rhs = v.Value;
        //    return lhs % rhs;
        //}

        //public static object operator *(Scalar sc, object v) {
        //    dynamic lhs = sc.Value;
        //    dynamic rhs = v;
        //    return lhs * rhs;
        //}

        //public static object operator *(object v, Scalar sc) {
        //    dynamic lhs = v;
        //    dynamic rhs = sc.Value;
        //    return lhs * rhs;
        //}

        //public static object operator *(Scalar sc, Scalar v) {
        //    dynamic lhs = sc.Value;
        //    dynamic rhs = v.Value;
        //    return lhs * rhs;
        //}

        //public static object operator &(Scalar sc, object v) {
        //    dynamic lhs = sc.Value;
        //    dynamic rhs = v;
        //    return lhs & rhs;
        //}

        //public static object operator &(object v, Scalar sc) {
        //    dynamic lhs = v;
        //    dynamic rhs = sc.Value;
        //    return lhs & rhs;
        //}

        //public static object operator &(Scalar sc, Scalar v) {
        //    dynamic lhs = sc.Value;
        //    dynamic rhs = v.Value;
        //    return lhs & rhs;
        //}

        //public static object operator |(Scalar sc, object v) {
        //    dynamic lhs = sc.Value;
        //    dynamic rhs = v;
        //    return lhs | rhs;
        //}

        //public static object operator |(object v, Scalar sc) {
        //    dynamic lhs = v;
        //    dynamic rhs = sc.Value;
        //    return lhs | rhs;
        //}

        //public static object operator |(Scalar sc, Scalar v) {
        //    dynamic lhs = sc.Value;
        //    dynamic rhs = v.Value;
        //    return lhs | rhs;
        //}


        //#region Null

        //public static object operator +(Scalar sc, NullScalar v) {
        //    return sc.Value;
        //}

        //public static object operator +(NullScalar v, Scalar sc) {
        //    return sc.Value;
        //}

        //public static object operator -(Scalar sc, NullScalar v) {
        //    return sc.Value;
        //}

        //public static object operator -(NullScalar v, Scalar sc) {
        //    return -sc;
        //}


        //public static object operator /(Scalar sc, NullScalar v) {
        //    return 0;
        //}

        //public static object operator /(NullScalar v, Scalar sc) {
        //    return 0;
        //}


        //public static object operator %(Scalar sc, NullScalar v) {
        //    return 0;
        //}

        //public static object operator %(NullScalar v, Scalar sc) {
        //    return 0;
        //}


        //public static object operator *(Scalar sc, NullScalar v) {
        //    return 0;
        //}

        //public static object operator *(NullScalar v, Scalar sc) {
        //    return 0;
        //}


        //public static object operator &(Scalar sc, NullScalar v) {
        //    dynamic lhs = sc.Value;
        //    return lhs & 0;
        //}

        //public static object operator &(NullScalar v, Scalar sc) {
        //    dynamic rhs = sc.Value;
        //    return 0 & rhs;
        //}

        //public static object operator |(Scalar sc, NullScalar v) {
        //    dynamic lhs = sc.Value;
        //    return lhs | 0;
        //}

        //public static object operator |(NullScalar v, Scalar sc) {
        //    dynamic rhs = sc.Value;
        //    return 0 | rhs;
        //}

        //#endregion

        //#endregion
    }
}