using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numpy;
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
        public void PythonIncludedTest()
        {
            var installer = new Installer();
            installer.SetupPython().Wait();
            Assert.IsTrue(Directory.Exists(installer.EmbeddedPythonHome));
        }

        [TestMethod]
        public void EmbeddedNumpyTest()
        {
            var numpy=NumPy.Instance;
            Console.WriteLine(numpy.self);
            dynamic sys=Py.Import("sys");
            Console.WriteLine(sys.version);
            //dynamic torch=Py.Import("torch");
            //Console.WriteLine(torch.version);
        }

    }
}
