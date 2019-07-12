using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace MxNet.gluon
{
    public class SymbolBlock : Base
    {
        public SymbolBlock(PyObject py)
        {
            __self__ = py;
        }
    }
}
