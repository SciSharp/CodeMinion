using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numpy;
using Numpy.Models;
using Assert = NUnit.Framework.Assert;

namespace Numpy.UnitTests
{
    [TestClass]
    public class NumpyConstants
    {
        [TestMethod]
        public void infTest()
        {
            //>>> np.inf
            //inf
            //>>> np.array([1]) / 0.
            //array([Inf])
            Console.WriteLine(np.inf);
            var x=np.array(1) / 0.0;
            Assert.AreEqual(np.array(np.inf), x);
            Assert.AreNotEqual(np.array(0f), x);
            Assert.AreEqual(float.PositiveInfinity, np.inf);
        }

        [TestMethod]
        public void ninfTest()
        {
            //>>> np.NINF
            //-inf
            //>>> np.log(0)
            //-inf
            Assert.AreEqual(-np.inf, np.NINF);
            Assert.AreEqual(-np.inf, (float)np.log((NDarray)0));
        }


    }
}
