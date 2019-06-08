using System;
using System.Collections.Generic;
using System.Text;
using Python.Runtime;

namespace Torch
{
    
    public partial class Dtype : PythonObject
    {
        public Dtype(PyObject pyobj) : base(pyobj)
        {
        }

        public bool is_floating_point => self.GetAttr("is_floating_point").As<bool>();
    }

    public partial class Device : PythonObject
    {
        public Device(PyObject pyobj) : base(pyobj)
        {
        }

    }

    public partial class Layout : PythonObject
    {
        public Layout(PyObject pyobj) : base(pyobj)
        {
        }

    }
}
