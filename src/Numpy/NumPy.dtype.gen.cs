using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Python.Runtime;
using Python.Included;
using NumSharp;

namespace Numpy
{
    public partial class NumPy
    {
        
        public Dtype bool_
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("bool_");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype bool8
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("bool8");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype @byte
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("byte");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype @short
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("short");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype intc
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("intc");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype int_
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("int_");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype longlong
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("longlong");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype intp
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("intp");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype int8
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("int8");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype int16
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("int16");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype int32
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("int32");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype int64
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("int64");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype ubyte
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("ubyte");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype @ushort
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("ushort");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype uintc
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("uintc");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype @uint
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("uint");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype ulonglong
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("ulonglong");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype uintp
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("uintp");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype uint8
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("uint8");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype uint16
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("uint16");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype uint32
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("uint32");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype uint64
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("uint64");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype half
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("half");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype single
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("single");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype @double
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("double");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype float_
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("float_");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype longfloat
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("longfloat");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype float16
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("float16");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype float32
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("float32");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype float64
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("float64");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype float96
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("float96");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype float128
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("float128");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype csingle
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("csingle");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype complex_
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("complex_");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype clongfloat
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("clongfloat");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype complex64
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("complex64");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype complex128
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("complex128");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype complex192
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("complex192");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype complex256
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("complex256");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype object_
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("object_");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype bytes_
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("bytes_");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype unicode_
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("unicode_");
                return ToCsharp<Dtype>(py);
            }
        }
        
        public Dtype @void
        {
            get
            {
                //auto-generated code, do not change
                dynamic py = self.GetAttr("void");
                return ToCsharp<Dtype>(py);
            }
        }
        
    }
}
