﻿using System;
using System.Collections.Generic;
using System.Text;
using Numpy.Models;

namespace Numpy
{

    public static partial class np
    {

        /// <summary>
        /// Calculate the absolute value element-wise.
        /// 
        /// np.abs is a shorthand for this function.
        /// </summary>
        /// <param name="x">
        /// Input array.
        /// </param>
        /// <param name="@out">
        /// A location into which the result is stored. If provided, it must have
        /// a shape that the inputs broadcast to. If not provided or None,
        /// a freshly-allocated array is returned. A tuple (possible only as a
        /// keyword argument) must have length equal to the number of outputs.
        /// </param>
        /// <param name="@where">
        /// Values of True indicate to calculate the ufunc at that position, values
        /// of False indicate to leave the value in the output alone.
        /// </param>
        /// <returns>
        /// An ndarray containing the absolute value of
        /// each element in x.  For complex input, a + ib, the
        /// absolute value is .
        /// This is a scalar if x is a scalar.
        /// </returns>
        public static NDarray abs(NDarray x, NDarray @out = null, NDarray @where = null)
            => NumPy.Instance.absolute(x, @out, @where);

        /// <summary>
        /// Return the minimum of an array or minimum along an axis.
        /// 
        /// Notes
        /// 
        /// NaN values are propagated, that is if at least one item is NaN, the
        /// corresponding min value will be NaN as well. To ignore NaN values
        /// (MATLAB behavior), please use nanmin.
        /// 
        /// Don’t use amin for element-wise comparison of 2 arrays; when
        /// a.shape[0] is 2, minimum(a[0], a[1]) is faster than
        /// amin(a, axis=0).
        /// </summary>
        /// <param name="a">
        /// Input data.
        /// </param>
        /// <param name="axis">
        /// Axis or axes along which to operate.  By default, flattened input is
        /// used.
        /// 
        /// If this is a tuple of ints, the minimum is selected over multiple axes,
        /// instead of a single axis or all the axes as before.
        /// </param>
        /// <param name="@out">
        /// Alternative output array in which to place the result.  Must
        /// be of the same shape and buffer length as the expected output.
        /// See doc.ufuncs (Section “Output arguments”) for more details.
        /// </param>
        /// <param name="keepdims">
        /// If this is set to True, the axes which are reduced are left
        /// in the result as dimensions with size one. With this option,
        /// the result will broadcast correctly against the input array.
        /// 
        /// If the default value is passed, then keepdims will not be
        /// passed through to the amin method of sub-classes of
        /// ndarray, however any non-default value will be.  If the
        /// sub-class’ method does not implement keepdims any
        /// exceptions will be raised.
        /// </param>
        /// <param name="initial">
        /// The maximum value of an output element. Must be present to allow
        /// computation on empty slice. See reduce for details.
        /// </param>
        /// <returns>
        /// Minimum of a. If axis is None, the result is a scalar value.
        /// If axis is given, the result is an array of dimension
        /// a.ndim - 1.
        /// </returns>
        public static NDarray min(NDarray a, int[] axis = null, NDarray @out = null, bool? keepdims = null, ValueType initial = null)
            => NumPy.Instance.amin(a, axis: axis, @out: @out, keepdims: keepdims, initial: initial);
    }
}
