using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numpy;
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
            var a = np.empty((2, 3), np.int32);
            Console.WriteLine(a);
            Assert.IsNotNull(a.ToString());            
        }

        [TestMethod]
        public void efficient_array_copy()
        {
            //var tensor = torch.empty((2, 3), dtype:dtype.Int32);
            //Console.WriteLine(tensor.ToString());
            //var storage=tensor.PyObject.storage();
            //Console.WriteLine("storage:"+storage);
            //long ptr = storage.data_ptr();
            //Console.WriteLine("ptr:"+ptr);
            //var array = new int[]{1, 2, 3, 4, 5, 6};
            //Marshal.Copy(array, 0, new IntPtr(ptr), array.Length);
            //Console.WriteLine(tensor.ToString());
            //Console.WriteLine("storage.is_pinned: " + storage.is_pinned());
            //Console.WriteLine("storage:" + storage);
        }
    }
}
