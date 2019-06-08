using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Python.Runtime;

namespace Torch
{
    // note: this file contains manually overridden API functions
    public static partial class torch
    {

        public static Dtype float32 => new Dtype( PyTorch.Instance.self.GetAttr("float32"));
        public static Dtype @float => new Dtype(PyTorch.Instance.self.GetAttr("float"));
        public static Dtype float64 => new Dtype(PyTorch.Instance.self.GetAttr("float64"));
        public static Dtype @double => new Dtype(PyTorch.Instance.self.GetAttr("double"));
        public static Dtype float16 => new Dtype(PyTorch.Instance.self.GetAttr("float16"));
        public static Dtype half => new Dtype(PyTorch.Instance.self.GetAttr("half"));
        public static Dtype uint8 => new Dtype(PyTorch.Instance.self.GetAttr("uint8"));
        public static Dtype int8 => new Dtype(PyTorch.Instance.self.GetAttr("int8"));
        public static Dtype int16 => new Dtype(PyTorch.Instance.self.GetAttr("int16"));
        public static Dtype @short => new Dtype(PyTorch.Instance.self.GetAttr("short"));
        public static Dtype int32 => new Dtype(PyTorch.Instance.self.GetAttr("int32"));
        public static Dtype @int => new Dtype(PyTorch.Instance.self.GetAttr("int"));
        public static Dtype int64 => new Dtype(PyTorch.Instance.self.GetAttr("int64"));
        public static Dtype @long => new Dtype(PyTorch.Instance.self.GetAttr("long"));



    }
}

