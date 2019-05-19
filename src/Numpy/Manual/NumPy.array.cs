using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Numpy;
using NumSharp;
using NumSharp.Utilities;
using Python.Runtime;

namespace Numpy
{
    /// <summary>
    /// Manual type conversions
    /// </summary>
    public partial class NumPy
    {

        public NDarray array(NDarray @object, Dtype dtype = null, bool? copy = null, string order = null, bool? subok = null, int? ndmin = null)
        {
            //auto-generated code, do not change
            var args = ToTuple(new object[]
            {
                @object,
            });
            var kwargs = new PyDict();
            if (dtype != null) kwargs["dtype"] = ToPython(dtype);
            if (copy != null) kwargs["copy"] = ToPython(copy);
            if (order != null) kwargs["order"] = ToPython(order);
            if (subok != null) kwargs["subok"] = ToPython(subok);
            if (ndmin != null) kwargs["ndmin"] = ToPython(ndmin);
            dynamic py = self.InvokeMethod("array", args, kwargs);
            return ToCsharp<NDarray>(py);
        }

        public NDarray<T> array<T>(T[] @object, Dtype dtype = null, bool? copy = null, string order = null, bool? subok = null, int? ndmin = null)
        {
            var type = @object.GetDtype();
            if (dtype != null && type.ToString() != dtype.ToString())
                throw new NotImplementedException("Type of the array is different from specified dtype. Data conversion is not supported (yet)");
            var ndarray = this.empty(new Shape(@object.Length), dtype: type, order:order); // todo: check out the other parameters
            long ptr = ndarray.PyObject.ctypes.data;
            switch ((object)@object)
            {
                case byte[] a: Marshal.Copy(a, 0, new IntPtr(ptr), a.Length); break;
                case short[] a: Marshal.Copy(a, 0, new IntPtr(ptr), a.Length); break;
                case int[] a: Marshal.Copy(a, 0, new IntPtr(ptr), a.Length); break;
                case long[] a: Marshal.Copy(a, 0, new IntPtr(ptr), a.Length); break;
                case float[] a: Marshal.Copy(a, 0, new IntPtr(ptr), a.Length); break;
                case double[] a: Marshal.Copy(a, 0, new IntPtr(ptr), a.Length); break;
            }
            return new NDarray<T>(ndarray);
        }
    }
}
