using System;
using System.Collections.Generic;

using System.Runtime.InteropServices;
using System.Text;
using Numpy.Models;
using NumSharp;
using Python.Runtime;


namespace Numpy
{
    public class NDarray : PythonObject
    {
        public NDarray(PyObject pyobj) : base(pyobj)
        {
        }

        public NDarray(NDarray t) : base((PyObject) t.PyObject)
        {
        }

        /// <summary>
        /// Returns a copy of the array data
        /// </summary>
        public T[] GetData<T>()
        {
            // note: this implementation works only for device CPU
            long ptr = PyObject.ctypes.data;
            int size = PyObject.size;
            object array = null;
            if (typeof(T) == typeof(byte)) array = new byte[size];
            else if (typeof(T) == typeof(short)) array = new short[size];
            else if (typeof(T) == typeof(int)) array = new int[size];
            else if (typeof(T) == typeof(long)) array = new long[size];
            else if (typeof(T) == typeof(float)) array = new float[size];
            else if (typeof(T) == typeof(double)) array = new double[size];
            else
                throw new InvalidOperationException(
                    "Can not copy the data with data type due to limitations of Marshal.Copy: " + typeof(T).Name);
            switch (array)
            {
                case byte[] a:
                    Marshal.Copy(new IntPtr(ptr), a, 0, a.Length);
                    break;
                case short[] a:
                    Marshal.Copy(new IntPtr(ptr), a, 0, a.Length);
                    break;
                case int[] a:
                    Marshal.Copy(new IntPtr(ptr), a, 0, a.Length);
                    break;
                case long[] a:
                    Marshal.Copy(new IntPtr(ptr), a, 0, a.Length);
                    break;
                case float[] a:
                    Marshal.Copy(new IntPtr(ptr), a, 0, a.Length);
                    break;
                case double[] a:
                    Marshal.Copy(new IntPtr(ptr), a, 0, a.Length);
                    break;
            }

            return (T[]) array;
        }

        /// <summary>
        /// Information about the memory layout of the array.
        /// </summary>
        public Flags flags => new Flags(self.GetAttr("flags")); // TODO: implement Flags

        /// <summary>
        /// Tuple of array dimensions.
        /// </summary>
        public Shape shape => self.GetAttr("shape").As<int[]>();

        /// <summary>
        /// Tuple of bytes to step in each dimension when traversing an array.
        /// </summary>
        public int[] strides => self.GetAttr("strides").As<int[]>();

        /// <summary>
        /// Number of array dimensions.
        /// </summary>
        public int ndim => self.GetAttr("ndim").As<int>();

        /// <summary>
        /// Python buffer object pointing to the start of the array’s data.
        /// </summary>
        public PyObject data => self.GetAttr("data");

        /// <summary>
        /// Number of elements in the array.
        /// </summary>
        public int size => self.GetAttr("size").As<int>();

        /// <summary>
        /// Length of one array element in bytes.
        /// </summary>
        public int itemsize => self.GetAttr("itemsize").As<int>();

        /// <summary>
        /// Total bytes consumed by the elements of the array.
        /// </summary>
        public int nbytes => self.GetAttr("nbytes").As<int>();

        /// <summary>
        /// Base object if memory is from some other object.
        /// </summary>
        public NDarray @base
        {
            get
            {
                PyObject base_obj = self.GetAttr("base");
                if (base_obj.IsNone())
                    return null;
                return new NDarray(base_obj);
            }
        }

        /// <summary>
        /// Data-type of the array’s elements.
        /// </summary>
        public Dtype dtype => new Dtype(self.GetAttr("dtype"));

        /// <summary>
        /// Same as self.transpose(), except that self is returned if self.ndim &lt; 2.
        /// </summary>
        public NDarray T => new NDarray(self.GetAttr("T"));

        /// <summary>
        /// The real part of the array.
        /// </summary>
        public NDarray real => new NDarray(self.GetAttr("real"));

        /// <summary>
        /// The imaginary part of the array.
        /// </summary>
        public NDarray imag => new NDarray(self.GetAttr("imag"));

        /// <summary>
        /// A 1-D iterator over the array.
        /// </summary>
        public PyObject flat => self.GetAttr("flat"); // todo: wrap and support usecases

        /// <summary>
        /// An object to simplify the interaction of the array with the ctypes module.
        /// </summary>
        public PyObject ctypes => self.GetAttr("ctypes"); // TODO: wrap ctypes


        /// <summary>
        /// Length of the array (same as size)
        /// </summary>
        public int len => self.InvokeMethod("__len__").As<int>();
    }

    public class NDarray<T> : NDarray
    {
        public NDarray(NDarray t) : base(t)
        {
        }

        public NDarray(PyObject pyobject) : base(pyobject)
        {
        }

        /// <summary>
        /// Returns a copy of the array data
        /// </summary>
        public T[] GetData()
        {
            return base.GetData<T>();
        }
    }
}
