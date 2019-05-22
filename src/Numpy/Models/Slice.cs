using System;
using System.Collections.Generic;
using System.Text;
using Python.Runtime;

namespace Numpy.Models
{
    public class Slice : PythonObject
    {
        public Slice(PyObject pyobject) : base(pyobject)
        {
        }

        // TODO: implement instantiation of a slice object in Python so that this object can be instantiated in C# 
    }
}
