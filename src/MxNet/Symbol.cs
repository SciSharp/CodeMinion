using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace MxNet
{
    public class Symbol : PythonObject
    {
        public Symbol(PyObject py) : base(py)
        {

        }
    }
}
