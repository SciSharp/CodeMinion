using System;
using System.Collections.Generic;

using System.Runtime.InteropServices;
using System.Text;
using Python.Runtime;


namespace Numpy
{
    public class NDarray : PythonObject
    {
        public NDarray(PyObject pyobj) : base(pyobj)
        {
        }

        public NDarray(NDarray t) : base((PyObject)t.PyObject)
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
            else throw new InvalidOperationException("Can not copy the data with data type due to limitations of Marshal.Copy: " + typeof(T).Name);
            switch (array)
            {
                case byte[] a: Marshal.Copy(new IntPtr(ptr), a, 0, a.Length); break;
                case short[] a: Marshal.Copy(new IntPtr(ptr), a, 0, a.Length); break;
                case int[] a: Marshal.Copy(new IntPtr(ptr), a, 0, a.Length); break;
                case long[] a: Marshal.Copy(new IntPtr(ptr), a, 0, a.Length); break;
                case float[] a: Marshal.Copy(new IntPtr(ptr), a, 0, a.Length); break;
                case double[] a: Marshal.Copy(new IntPtr(ptr), a, 0, a.Length); break;
            }
            return (T[])array;
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
