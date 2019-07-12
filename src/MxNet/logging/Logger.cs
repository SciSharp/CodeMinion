using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace MxNet.logging
{
    public class Logger : Base
    {
        public Logger(PyObject py)
        {
            __self__ = py;
        }
    }
}
