using Python.Runtime;

namespace Torch
{
    public partial class Device : PythonObject
    {
        public Device(PyObject pyobj) : base(pyobj)
        {
        }

    }
}