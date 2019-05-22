using System;
using System.Collections.Generic;
using System.Text;
using Python.Runtime;

namespace Numpy
{
    public class PythonObject : IDisposable
    {
        protected readonly PyObject _pobj;
        public dynamic PyObject => _pobj;

        public IntPtr Handle => _pobj.Handle;

        public PythonObject(PyObject pyobject)
        {
            this._pobj = pyobject;
        }

        public PythonObject(PythonObject t)
        {
            this._pobj = t.PyObject;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case PythonObject other:
                    return _pobj.Equals(other._pobj);
                case PyObject other:
                    return _pobj.Equals(other);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _pobj.GetHashCode();
        }

        public override string ToString()
        {
            return _pobj.ToString();
        }

        public void Dispose()
        {
            _pobj?.Dispose();
        }
    }
}
