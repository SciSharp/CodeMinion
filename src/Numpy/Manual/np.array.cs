using System;
using System.Collections.Generic;
using System.Text;

namespace Numpy
{

    public static partial class np
    {

        /// <summary>
        /// Create an array.
        /// 
        /// <param name="data">
        /// The array to initialize the ndarray with
        /// </param>
        /// <returns>
        /// An array object satisfying the specified requirements.
        /// </returns>
        public static NDarray<T> array<T>(params T[] data)
        {
            return NumPy.Instance.array(data);
        }

        public static NDarray<T> array<T>(T[,,] data, Dtype dtype = null, bool? copy = null, string order = null, bool? subok = null, int? ndmin = null)
        {
            return NumPy.Instance.array(data, dtype, copy, order, subok, ndmin);
        }
    }
}
