using System;
using System.Collections.Generic;
using System.Text;

namespace Numpy
{
    public static partial class np
    {
        /// <summary>
        /// IEEE 754 floating point representation of (positive) infinity.
        /// </summary>
        public static float inf => NumPy.Instance.self.GetAttr("inf").As<float>();

        /// <summary>
        /// IEEE 754 floating point representation of (positive) infinity.
        /// 
        /// Use np.inf because Inf, Infinity, PINF and infty are aliases for inf.For more details, see inf.
        /// </summary>
        public static float Inf => NumPy.Instance.self.GetAttr("inf").As<float>();

        /// <summary>
        /// IEEE 754 floating point representation of (positive) infinity.
        /// 
        /// Use np.inf because Inf, Infinity, PINF and infty are aliases for inf.For more details, see inf.
        /// </summary>
        public static float Infinity => NumPy.Instance.self.GetAttr("inf").As<float>();

        /// <summary>
        /// IEEE 754 floating point representation of (positive) infinity.
        /// 
        /// Use np.inf because Inf, Infinity, PINF and infty are aliases for inf.For more details, see inf.
        /// </summary>
        public static float PINF => NumPy.Instance.self.GetAttr("inf").As<float>();

        /// <summary>
        /// IEEE 754 floating point representation of (positive) infinity.
        /// 
        /// Use np.inf because Inf, Infinity, PINF and infty are aliases for inf.For more details, see inf.
        /// </summary>
        public static float infty => NumPy.Instance.self.GetAttr("inf").As<float>();

        /// <summary>
        /// IEEE 754 floating point representation of (positive) infinity.
        /// </summary>
        public static float NINF => NumPy.Instance.self.GetAttr("NINF").As<float>();

        /// <summary>
        /// IEEE 754 floating point representation of Not a Number(NaN).
        /// </summary>
        public static float nan => NumPy.Instance.self.GetAttr("nan").As<float>();

        /// <summary>
        /// IEEE 754 floating point representation of Not a Number(NaN).
        /// 
        /// NaN and NAN are equivalent definitions of nan.Please use nan instead of NAN.
        /// </summary>
        public static float NaN => NumPy.Instance.self.GetAttr("nan").As<float>();

        /// <summary>
        /// IEEE 754 floating point representation of Not a Number(NaN).
        /// 
        /// NaN and NAN are equivalent definitions of nan.Please use nan instead of NAN.
        /// </summary>
        public static float NAN => NumPy.Instance.self.GetAttr("nan").As<float>();
    }
}
