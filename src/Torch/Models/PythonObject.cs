﻿using System;
using System.Collections.Generic;
using System.Text;
using Python.Runtime;

namespace Torch
{
    public partial class PythonObject : IDisposable
    {
        protected readonly PyObject self;
        public dynamic PyObject => self;

        public IntPtr Handle => self.Handle;

        public PythonObject(PyObject pyobject)
        {
            this.self = pyobject;
        }

        public PythonObject(Tensor t)
        {
            this.self = t.PyObject;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case PythonObject other:
                    return self.Equals(other.self);
                case PyObject other:
                    return self.Equals(other);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return self.GetHashCode();
        }

        // there is no need for this yet. if it is, we'll generate it automatically
        public object FromPython(PyObject obj)
        {
            if (obj.IsNone())
                return null;
            var python_typename = Runtime.PyObject_GetTypeName(obj.Handle);
            switch (python_typename)
            {
                case "Tensor": return new Tensor(obj);
                default: throw new NotImplementedException($"Type is not yet supported: { python_typename}. Add it to 'FromPythonConversions'");
            }
            return obj;
        }

        public string repr => ToString();

        public override string ToString()
        {
            return self.ToString();
        }

        public void Dispose()
        {
            self?.Dispose();
        }
    }
}
