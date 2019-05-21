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
        public Flags flags => new Flags(_pobj.GetAttr("flags"));

        /// <summary>
        /// Tuple of array dimensions.
        /// </summary>
        public Shape shape => _pobj.GetAttr("shape").As<int[]>();

        /// <summary>
        /// Tuple of bytes to step in each dimension when traversing an array.
        /// </summary>
        public int[] strides => _pobj.GetAttr("strides").As<int[]>();

        /// <summary>
        /// Number of array dimensions.
        /// </summary>
        public int ndim => _pobj.GetAttr("ndim").As<int>();

        /// <summary>
        /// Python buffer object pointing to the start of the array’s data.
        /// </summary>
        public PyObject data => _pobj.GetAttr("data");

        /// <summary>
        /// Number of elements in the array.
        /// </summary>
        public int size => _pobj.GetAttr("size").As<int>();

        /// <summary>
        /// Length of one array element in bytes.
        /// </summary>
        public int itemsize => _pobj.GetAttr("itemsize").As<int>();

        /// <summary>
        /// Total bytes consumed by the elements of the array.
        /// </summary>
        public int nbytes => _pobj.GetAttr("nbytes").As<int>();

        /// <summary>
        /// Base object if memory is from some other object.
        /// </summary>
        public NDarray @base
        {
            get
            {
                dynamic base_obj = _pobj.GetAttr("base");
                var type = base_obj.__class__.ToString();
                if (type== "<class 'NoneType'>")
                    return null;
                return new NDarray(base_obj);
            }
        }
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
