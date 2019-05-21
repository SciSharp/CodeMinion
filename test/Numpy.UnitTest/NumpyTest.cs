using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numpy;
using NumSharp;
using Python.Included;
using Python.Runtime;
using Assert = NUnit.Framework.Assert;

namespace Numpy.UnitTests
{
    [TestClass]
    public class NumpyTest
    {
        [TestMethod]
        public void empty()
        {
            // initialize an array with random integers
            var a = np.empty((2, 3), np.int32);
            Console.WriteLine(a);
            Assert.IsNotNull(a.ToString());
            // this should print out the exact integers of the array
            foreach(var x in a.GetData<int>())
                Console.WriteLine(x);             
        }

        [TestMethod]
        public void efficient_array_copy()
        {
            var a = np.empty((2, 3), np.int32);
            Console.WriteLine(a);
            Assert.IsNotNull(a.ToString());
            long ptr = a.PyObject.ctypes.data;
            Console.WriteLine("ptr: " + ptr);
            var array = new int[]{1, 2, 3, 4, 5, 6};
            Marshal.Copy(array, 0, new IntPtr(ptr), array.Length);
            Console.WriteLine(a.ToString());
        }

        [TestMethod]
        public void array()
        {
            var array = new int[] { 1, 2, 3, 4, 5, 6 };
            var a = np.array(array);
            Console.WriteLine(a);
            Assert.AreEqual(array, a.GetData());
        }    

        [TestMethod]
        public void EmbeddedNumpyTest()
        {
            var numpy=NumPy.Instance;
            Console.WriteLine(numpy.self);
            dynamic sys=Py.Import("sys");
            Console.WriteLine(sys.version);
        }

        [TestMethod]
        public void NonEmbeddedNumpyTest()
        {
            PythonEnv.DeployEmbeddedPython = false;
            var numpy = NumPy.Instance;
            Console.WriteLine(numpy.self);
            dynamic sys = Py.Import("sys");
            Console.WriteLine(sys.version);
        }

        [TestMethod]
        public void ndarray_shape()
        {
            var array = new int[] { 1, 2, 3, 4, 5, 6 };
            var a = np.array(array);
            Assert.AreEqual(new Shape(6), a.shape);
            Assert.AreEqual(new Shape(100), np.arange(100).shape);
        }

        [TestMethod]
        public void ndarray_strides()
        {
            Assert.AreEqual(new int[]{4}, np.array(new int[] { 1, 2, 3, 4, 5, 6 }).strides);
            Assert.AreEqual(new int[] { 8 }, np.arange(10, dtype: np.longlong).strides);
        }

        [TestMethod]
        public void ndarray_ndim()
        {
            Assert.AreEqual(1, np.array(new int[] { 1, 2, 3, 4, 5, 6 }).ndim);
            Assert.AreEqual(1, np.arange(10, dtype: np.longlong).ndim);
        }

        [TestMethod]
        public void ndarray_size()
        {
            Assert.AreEqual(6, np.array(new int[] { 1, 2, 3, 4, 5, 6 }).size);
            Assert.AreEqual(10, np.arange(10, dtype: np.longlong).size);
        }

        [TestMethod]
        public void ndarray_itemsize()
        {
            Assert.AreEqual( 4 , np.array(new int[] { 1, 2, 3, 4, 5, 6 }).itemsize);
            Assert.AreEqual( 8 , np.arange(10, dtype: np.longlong).itemsize);
        }

        [TestMethod]
        public void ndarray_nbytes()
        {
            Assert.AreEqual(24, np.array(new int[] { 1, 2, 3, 4, 5, 6 }).nbytes);
            Assert.AreEqual(80, np.arange(10, dtype: np.longlong).nbytes);
        }

        [TestMethod]
        public void ndarray_base()
        {
            var a = np.array(new int[] {1, 2, 3, 4, 5, 6});
            var b = np.array(a);
            Assert.AreEqual(null, a.@base);
            Assert.AreEqual(a, b.@base);
        }
    }
}
