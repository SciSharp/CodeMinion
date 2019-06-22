using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Python.Runtime;

namespace Torch.nn
{
    [TestClass]
    public class NN_tests : BaseTestCase
    {
        [TestMethod]
        public void Parameter()
        {
            var Parameter = PyTorch.Instance.self.GetAttr("nn").GetAttr("Parameter");
            Console.WriteLine(Parameter.ToString());
            var x = torch.tensor(new double[] { 1, 2, 3 });
            var p = Parameter(x.PyObject, requires_grad: false);
            var p1 = (PyTorch.Instance.self.GetAttr("nn") as PyObject).InvokeMethod("Parameter", new PyTuple(new PyObject[] { x.PyObject }), Py.kw("requires_grad", new PyObject(Runtime.PyFalse)));
            Console.WriteLine(p.ToString());
            Console.WriteLine(p1.ToString());
            // 
            var p2 = new torch.nn.Parameter(x, true);
            Console.WriteLine(p2.ToString());
        }
    }
}
