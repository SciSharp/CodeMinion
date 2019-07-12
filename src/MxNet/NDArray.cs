using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace MxNet
{
    public class NDArray : PythonObject
    {
        public NDArray(PyObject py) : base(py)
        {

        }
    }
}
