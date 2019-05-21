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
        
        public NDarray empty(NumSharp.Shape shape, Dtype dtype = null, string order = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                shape,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (order!=null) kwargs["order"]=ToPython(order);
            dynamic py = self.InvokeMethod("empty", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray empty_like(NDarray prototype, Dtype dtype = null, string order = null, bool? subok = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                prototype,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (order!=null) kwargs["order"]=ToPython(order);
            if (subok!=null) kwargs["subok"]=ToPython(subok);
            dynamic py = self.InvokeMethod("empty_like", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray<T> empty_like<T>(T[] prototype, Dtype dtype = null, string order = null, bool? subok = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                prototype,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (order!=null) kwargs["order"]=ToPython(order);
            if (subok!=null) kwargs["subok"]=ToPython(subok);
            dynamic py = self.InvokeMethod("empty_like", args, kwargs);
            return ToCsharp<NDarray<T>>(py);
        }
        
        public NDarray eye(int N, int? M = null, int? k = null, Dtype dtype = null, string order = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                N,
            });
            var kwargs=new PyDict();
            if (M!=null) kwargs["M"]=ToPython(M);
            if (k!=null) kwargs["k"]=ToPython(k);
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (order!=null) kwargs["order"]=ToPython(order);
            dynamic py = self.InvokeMethod("eye", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray identity(int n, Dtype dtype = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                n,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            dynamic py = self.InvokeMethod("identity", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray ones(NumSharp.Shape shape, Dtype dtype = null, string order = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                shape,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (order!=null) kwargs["order"]=ToPython(order);
            dynamic py = self.InvokeMethod("ones", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray ones_like(NDarray a, Dtype dtype = null, string order = null, bool? subok = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                a,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (order!=null) kwargs["order"]=ToPython(order);
            if (subok!=null) kwargs["subok"]=ToPython(subok);
            dynamic py = self.InvokeMethod("ones_like", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray<T> ones_like<T>(T[] a, Dtype dtype = null, string order = null, bool? subok = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                a,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (order!=null) kwargs["order"]=ToPython(order);
            if (subok!=null) kwargs["subok"]=ToPython(subok);
            dynamic py = self.InvokeMethod("ones_like", args, kwargs);
            return ToCsharp<NDarray<T>>(py);
        }
        
        public NDarray zeros(NumSharp.Shape shape, Dtype dtype = null, string order = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                shape,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (order!=null) kwargs["order"]=ToPython(order);
            dynamic py = self.InvokeMethod("zeros", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray zeros_like(NDarray a, Dtype dtype = null, string order = null, bool? subok = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                a,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (order!=null) kwargs["order"]=ToPython(order);
            if (subok!=null) kwargs["subok"]=ToPython(subok);
            dynamic py = self.InvokeMethod("zeros_like", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray<T> zeros_like<T>(T[] a, Dtype dtype = null, string order = null, bool? subok = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                a,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (order!=null) kwargs["order"]=ToPython(order);
            if (subok!=null) kwargs["subok"]=ToPython(subok);
            dynamic py = self.InvokeMethod("zeros_like", args, kwargs);
            return ToCsharp<NDarray<T>>(py);
        }
        
        public NDarray full(NumSharp.Shape shape, ValueType fill_value, Dtype dtype = null, string order = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                shape,
                fill_value,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (order!=null) kwargs["order"]=ToPython(order);
            dynamic py = self.InvokeMethod("full", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray full_like(NDarray a, ValueType fill_value, Dtype dtype = null, string order = null, bool? subok = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                a,
                fill_value,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (order!=null) kwargs["order"]=ToPython(order);
            if (subok!=null) kwargs["subok"]=ToPython(subok);
            dynamic py = self.InvokeMethod("full_like", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray<T> full_like<T>(T[] a, ValueType fill_value, Dtype dtype = null, string order = null, bool? subok = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                a,
                fill_value,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (order!=null) kwargs["order"]=ToPython(order);
            if (subok!=null) kwargs["subok"]=ToPython(subok);
            dynamic py = self.InvokeMethod("full_like", args, kwargs);
            return ToCsharp<NDarray<T>>(py);
        }
        
        public NDarray asarray(NDarray a, Dtype dtype = null, string order = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                a,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (order!=null) kwargs["order"]=ToPython(order);
            dynamic py = self.InvokeMethod("asarray", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray<T> asarray<T>(T[] a, Dtype dtype = null, string order = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                a,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (order!=null) kwargs["order"]=ToPython(order);
            dynamic py = self.InvokeMethod("asarray", args, kwargs);
            return ToCsharp<NDarray<T>>(py);
        }
        
        public NDarray asanyarray(NDarray a, Dtype dtype = null, string order = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                a,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (order!=null) kwargs["order"]=ToPython(order);
            dynamic py = self.InvokeMethod("asanyarray", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray<T> asanyarray<T>(T[] a, Dtype dtype = null, string order = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                a,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (order!=null) kwargs["order"]=ToPython(order);
            dynamic py = self.InvokeMethod("asanyarray", args, kwargs);
            return ToCsharp<NDarray<T>>(py);
        }
        
        public NDarray ascontiguousarray(NDarray a, Dtype dtype = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                a,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            dynamic py = self.InvokeMethod("ascontiguousarray", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray<T> ascontiguousarray<T>(T[] a, Dtype dtype = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                a,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            dynamic py = self.InvokeMethod("ascontiguousarray", args, kwargs);
            return ToCsharp<NDarray<T>>(py);
        }
        
        public matrix asmatrix(NDarray data, Dtype dtype)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                data,
                dtype,
            });
            var kwargs=new PyDict();
            dynamic py = self.InvokeMethod("asmatrix", args, kwargs);
            return ToCsharp<matrix>(py);
        }
        
        public matrix asmatrix<T>(T[] data, Dtype dtype)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                data,
                dtype,
            });
            var kwargs=new PyDict();
            dynamic py = self.InvokeMethod("asmatrix", args, kwargs);
            return ToCsharp<matrix>(py);
        }
        
        public NDarray copy(NDarray a, string order = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                a,
            });
            var kwargs=new PyDict();
            if (order!=null) kwargs["order"]=ToPython(order);
            dynamic py = self.InvokeMethod("copy", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray<T> copy<T>(T[] a, string order = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                a,
            });
            var kwargs=new PyDict();
            if (order!=null) kwargs["order"]=ToPython(order);
            dynamic py = self.InvokeMethod("copy", args, kwargs);
            return ToCsharp<NDarray<T>>(py);
        }
        
        /*
        public void frombuffer(buffer_like buffer, Dtype dtype = null, int? count = null, int? offset = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                buffer,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (count!=null) kwargs["count"]=ToPython(count);
            if (offset!=null) kwargs["offset"]=ToPython(offset);
            dynamic py = self.InvokeMethod("frombuffer", args, kwargs);
        }
        */
        
        public void fromfile(string file, Dtype dtype, int count, string sep)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                file,
                dtype,
                count,
                sep,
            });
            var kwargs=new PyDict();
            dynamic py = self.InvokeMethod("fromfile", args, kwargs);
        }
        
        public object fromfunction(Delegate function, NumSharp.Shape shape, Dtype dtype = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                function,
                shape,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            dynamic py = self.InvokeMethod("fromfunction", args, kwargs);
            return ToCsharp<object>(py);
        }
        
        public NDarray<T> fromiter<T>(IEnumerable<T> iterable, Dtype dtype, int? count = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                iterable,
                dtype,
            });
            var kwargs=new PyDict();
            if (count!=null) kwargs["count"]=ToPython(count);
            dynamic py = self.InvokeMethod("fromiter", args, kwargs);
            return ToCsharp<NDarray<T>>(py);
        }
        
        public NDarray fromstring(string @string, Dtype dtype = null, int? count = null, string sep = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                @string,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (count!=null) kwargs["count"]=ToPython(count);
            if (sep!=null) kwargs["sep"]=ToPython(sep);
            dynamic py = self.InvokeMethod("fromstring", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray loadtxt(string fname, Dtype dtype = null, string[] comments = null, string delimiter = null, Hashtable converters = null, int? skiprows = null, int[] usecols = null, bool? unpack = null, int? ndmin = null, string encoding = null, int? max_rows = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                fname,
            });
            var kwargs=new PyDict();
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (comments!=null) kwargs["comments"]=ToPython(comments);
            if (delimiter!=null) kwargs["delimiter"]=ToPython(delimiter);
            if (converters!=null) kwargs["converters"]=ToPython(converters);
            if (skiprows!=null) kwargs["skiprows"]=ToPython(skiprows);
            if (usecols!=null) kwargs["usecols"]=ToPython(usecols);
            if (unpack!=null) kwargs["unpack"]=ToPython(unpack);
            if (ndmin!=null) kwargs["ndmin"]=ToPython(ndmin);
            if (encoding!=null) kwargs["encoding"]=ToPython(encoding);
            if (max_rows!=null) kwargs["max_rows"]=ToPython(max_rows);
            dynamic py = self.InvokeMethod("loadtxt", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public void asarray(string[] obj, int? itemsize = null, bool? unicode = null, string order = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                obj,
            });
            var kwargs=new PyDict();
            if (itemsize!=null) kwargs["itemsize"]=ToPython(itemsize);
            if (unicode!=null) kwargs["unicode"]=ToPython(unicode);
            if (order!=null) kwargs["order"]=ToPython(order);
            dynamic py = self.InvokeMethod("asarray", args, kwargs);
        }
        
        public NDarray arange(byte start, byte stop, byte step = 1, Dtype dtype = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                start,
                stop,
            });
            var kwargs=new PyDict();
            if (step!=null) kwargs["step"]=ToPython(step);
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            dynamic py = self.InvokeMethod("arange", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray arange(byte stop, byte step = 1, Dtype dtype = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                stop,
            });
            var kwargs=new PyDict();
            if (step!=null) kwargs["step"]=ToPython(step);
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            dynamic py = self.InvokeMethod("arange", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray arange(short start, short stop, short step = 1, Dtype dtype = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                start,
                stop,
            });
            var kwargs=new PyDict();
            if (step!=null) kwargs["step"]=ToPython(step);
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            dynamic py = self.InvokeMethod("arange", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray arange(short stop, short step = 1, Dtype dtype = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                stop,
            });
            var kwargs=new PyDict();
            if (step!=null) kwargs["step"]=ToPython(step);
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            dynamic py = self.InvokeMethod("arange", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray arange(int start, int stop, int step = 1, Dtype dtype = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                start,
                stop,
            });
            var kwargs=new PyDict();
            if (step!=null) kwargs["step"]=ToPython(step);
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            dynamic py = self.InvokeMethod("arange", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray arange(int stop, int step = 1, Dtype dtype = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                stop,
            });
            var kwargs=new PyDict();
            if (step!=null) kwargs["step"]=ToPython(step);
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            dynamic py = self.InvokeMethod("arange", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray arange(long start, long stop, long step = 1, Dtype dtype = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                start,
                stop,
            });
            var kwargs=new PyDict();
            if (step!=null) kwargs["step"]=ToPython(step);
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            dynamic py = self.InvokeMethod("arange", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray arange(long stop, long step = 1, Dtype dtype = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                stop,
            });
            var kwargs=new PyDict();
            if (step!=null) kwargs["step"]=ToPython(step);
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            dynamic py = self.InvokeMethod("arange", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray arange(float start, float stop, float step = 1, Dtype dtype = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                start,
                stop,
            });
            var kwargs=new PyDict();
            if (step!=null) kwargs["step"]=ToPython(step);
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            dynamic py = self.InvokeMethod("arange", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray arange(float stop, float step = 1, Dtype dtype = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                stop,
            });
            var kwargs=new PyDict();
            if (step!=null) kwargs["step"]=ToPython(step);
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            dynamic py = self.InvokeMethod("arange", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray arange(double start, double stop, double step = 1, Dtype dtype = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                start,
                stop,
            });
            var kwargs=new PyDict();
            if (step!=null) kwargs["step"]=ToPython(step);
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            dynamic py = self.InvokeMethod("arange", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray arange(double stop, double step = 1, Dtype dtype = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                stop,
            });
            var kwargs=new PyDict();
            if (step!=null) kwargs["step"]=ToPython(step);
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            dynamic py = self.InvokeMethod("arange", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        // Error generating delaration: linspace
        // Message: Return tuple
        /*
           at CodeMinion.Core.CodeGenerator.GenerateReturnType(Declaration decl) in D:\dev\CodeMinion\src\CodeMinion.Core\CodeGenerator.cs:line 253
   at CodeMinion.Core.CodeGenerator.GenerateApiFunction(Declaration decl, CodeWriter s) in D:\dev\CodeMinion\src\CodeMinion.Core\CodeGenerator.cs:line 57
   at CodeMinion.Core.CodeGenerator.<>c__DisplayClass33_0.<GenerateApiImpl>b__1() in D:\dev\CodeMinion\src\CodeMinion.Core\CodeGenerator.cs:line 449
        ----------------------------
        Declaration JSON:
        {
  "Arguments": [
    {
      "IsNullable": false,
      "IsValueType": false,
      "Name": "start",
      "Type": "NDarray",
      "DefaultValue": null,
      "IsNamedArg": false,
      "Description": "The starting value of the sequence."
    },
    {
      "IsNullable": false,
      "IsValueType": false,
      "Name": "stop",
      "Type": "NDarray",
      "DefaultValue": null,
      "IsNamedArg": false,
      "Description": "The end value of the sequence, unless endpoint is set to False.\nIn that case, the sequence consists of all but the last of num + 1\nevenly spaced samples, so that stop is excluded.  Note that the step\nsize changes when endpoint is False."
    },
    {
      "IsNullable": true,
      "IsValueType": false,
      "Name": "num",
      "Type": "int",
      "DefaultValue": null,
      "IsNamedArg": true,
      "Description": "Number of samples to generate. Default is 50. Must be non-negative."
    },
    {
      "IsNullable": true,
      "IsValueType": false,
      "Name": "endpoint",
      "Type": "bool",
      "DefaultValue": null,
      "IsNamedArg": true,
      "Description": "If True, stop is the last sample. Otherwise, it is not included.\nDefault is True."
    },
    {
      "IsNullable": true,
      "IsValueType": false,
      "Name": "retstep",
      "Type": "bool",
      "DefaultValue": null,
      "IsNamedArg": true,
      "Description": "If True, return (samples, step), where step is the spacing\nbetween samples."
    },
    {
      "IsNullable": true,
      "IsValueType": false,
      "Name": "dtype",
      "Type": "Dtype",
      "DefaultValue": null,
      "IsNamedArg": true,
      "Description": "The type of the output array.  If dtype is not given, infer the data\ntype from the other input arguments."
    },
    {
      "IsNullable": true,
      "IsValueType": false,
      "Name": "axis",
      "Type": "int",
      "DefaultValue": null,
      "IsNamedArg": true,
      "Description": "The axis in the result to store the samples.  Relevant only if start\nor stop are array-like.  By default (0), the samples will be along a\nnew axis inserted at the beginning. Use -1 to get an axis at the end."
    }
  ],
  "Generics": null,
  "Name": "linspace",
  "ClassName": "numpy",
  "Returns": [
    {
      "IsNullable": false,
      "IsValueType": false,
      "Name": "samples",
      "Type": "NDarray",
      "DefaultValue": null,
      "IsNamedArg": false,
      "Description": "There are num equally spaced samples in the closed interval\n[start, stop] or the half-open interval [start, stop)\n(depending on whether endpoint is True or False)."
    },
    {
      "IsNullable": false,
      "IsValueType": false,
      "Name": "step",
      "Type": "float",
      "DefaultValue": null,
      "IsNamedArg": false,
      "Description": "Only returned if retstep is True\r\n\r\nSize of spacing between samples."
    }
  ],
  "IsDeprecated": false,
  "ManualOverride": false,
  "CommentOut": false,
  "DebuggerBreak": false,
  "Description": "Return evenly spaced numbers over a specified interval.\r\n\r\nReturns num evenly spaced samples, calculated over the\ninterval [start, stop].\r\n\r\nThe endpoint of the interval can optionally be excluded."
}
        */
        // Error generating delaration: linspace
        // Message: Return tuple
        /*
           at CodeMinion.Core.CodeGenerator.GenerateReturnType(Declaration decl) in D:\dev\CodeMinion\src\CodeMinion.Core\CodeGenerator.cs:line 253
   at CodeMinion.Core.CodeGenerator.GenerateApiFunction(Declaration decl, CodeWriter s) in D:\dev\CodeMinion\src\CodeMinion.Core\CodeGenerator.cs:line 57
   at CodeMinion.Core.CodeGenerator.<>c__DisplayClass33_0.<GenerateApiImpl>b__1() in D:\dev\CodeMinion\src\CodeMinion.Core\CodeGenerator.cs:line 449
        ----------------------------
        Declaration JSON:
        {
  "Arguments": [
    {
      "IsNullable": false,
      "IsValueType": false,
      "Name": "start",
      "Type": "T[]",
      "DefaultValue": null,
      "IsNamedArg": false,
      "Description": "The starting value of the sequence."
    },
    {
      "IsNullable": false,
      "IsValueType": false,
      "Name": "stop",
      "Type": "T[]",
      "DefaultValue": null,
      "IsNamedArg": false,
      "Description": "The end value of the sequence, unless endpoint is set to False.\nIn that case, the sequence consists of all but the last of num + 1\nevenly spaced samples, so that stop is excluded.  Note that the step\nsize changes when endpoint is False."
    },
    {
      "IsNullable": true,
      "IsValueType": false,
      "Name": "num",
      "Type": "int",
      "DefaultValue": null,
      "IsNamedArg": true,
      "Description": "Number of samples to generate. Default is 50. Must be non-negative."
    },
    {
      "IsNullable": true,
      "IsValueType": false,
      "Name": "endpoint",
      "Type": "bool",
      "DefaultValue": null,
      "IsNamedArg": true,
      "Description": "If True, stop is the last sample. Otherwise, it is not included.\nDefault is True."
    },
    {
      "IsNullable": true,
      "IsValueType": false,
      "Name": "retstep",
      "Type": "bool",
      "DefaultValue": null,
      "IsNamedArg": true,
      "Description": "If True, return (samples, step), where step is the spacing\nbetween samples."
    },
    {
      "IsNullable": true,
      "IsValueType": false,
      "Name": "dtype",
      "Type": "Dtype",
      "DefaultValue": null,
      "IsNamedArg": true,
      "Description": "The type of the output array.  If dtype is not given, infer the data\ntype from the other input arguments."
    },
    {
      "IsNullable": true,
      "IsValueType": false,
      "Name": "axis",
      "Type": "int",
      "DefaultValue": null,
      "IsNamedArg": true,
      "Description": "The axis in the result to store the samples.  Relevant only if start\nor stop are array-like.  By default (0), the samples will be along a\nnew axis inserted at the beginning. Use -1 to get an axis at the end."
    }
  ],
  "Generics": [
    "T"
  ],
  "Name": "linspace",
  "ClassName": "numpy",
  "Returns": [
    {
      "IsNullable": false,
      "IsValueType": false,
      "Name": "samples",
      "Type": "NDarray<T>",
      "DefaultValue": null,
      "IsNamedArg": false,
      "Description": "There are num equally spaced samples in the closed interval\n[start, stop] or the half-open interval [start, stop)\n(depending on whether endpoint is True or False)."
    },
    {
      "IsNullable": false,
      "IsValueType": false,
      "Name": "step",
      "Type": "float",
      "DefaultValue": null,
      "IsNamedArg": false,
      "Description": "Only returned if retstep is True\r\n\r\nSize of spacing between samples."
    }
  ],
  "IsDeprecated": false,
  "ManualOverride": false,
  "CommentOut": false,
  "DebuggerBreak": false,
  "Description": "Return evenly spaced numbers over a specified interval.\r\n\r\nReturns num evenly spaced samples, calculated over the\ninterval [start, stop].\r\n\r\nThe endpoint of the interval can optionally be excluded."
}
        */
        public NDarray logspace(NDarray start, NDarray stop, int? num = null, bool? endpoint = null, float? @base = null, Dtype dtype = null, int? axis = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                start,
                stop,
            });
            var kwargs=new PyDict();
            if (num!=null) kwargs["num"]=ToPython(num);
            if (endpoint!=null) kwargs["endpoint"]=ToPython(endpoint);
            if (@base!=null) kwargs["base"]=ToPython(@base);
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (axis!=null) kwargs["axis"]=ToPython(axis);
            dynamic py = self.InvokeMethod("logspace", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray<T> logspace<T>(T[] start, T[] stop, int? num = null, bool? endpoint = null, float? @base = null, Dtype dtype = null, int? axis = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                start,
                stop,
            });
            var kwargs=new PyDict();
            if (num!=null) kwargs["num"]=ToPython(num);
            if (endpoint!=null) kwargs["endpoint"]=ToPython(endpoint);
            if (@base!=null) kwargs["base"]=ToPython(@base);
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (axis!=null) kwargs["axis"]=ToPython(axis);
            dynamic py = self.InvokeMethod("logspace", args, kwargs);
            return ToCsharp<NDarray<T>>(py);
        }
        
        public NDarray geomspace(NDarray start, NDarray stop, int? num = null, bool? endpoint = null, Dtype dtype = null, int? axis = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                start,
                stop,
            });
            var kwargs=new PyDict();
            if (num!=null) kwargs["num"]=ToPython(num);
            if (endpoint!=null) kwargs["endpoint"]=ToPython(endpoint);
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (axis!=null) kwargs["axis"]=ToPython(axis);
            dynamic py = self.InvokeMethod("geomspace", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray<T> geomspace<T>(T[] start, T[] stop, int? num = null, bool? endpoint = null, Dtype dtype = null, int? axis = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                start,
                stop,
            });
            var kwargs=new PyDict();
            if (num!=null) kwargs["num"]=ToPython(num);
            if (endpoint!=null) kwargs["endpoint"]=ToPython(endpoint);
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            if (axis!=null) kwargs["axis"]=ToPython(axis);
            dynamic py = self.InvokeMethod("geomspace", args, kwargs);
            return ToCsharp<NDarray<T>>(py);
        }
        
        /*
        public NDarray meshgrid(NDarray x1, x2,…, xn, {‘xy’ indexing = null, bool? sparse = null, bool? copy = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                x1, x2,…, xn,
            });
            var kwargs=new PyDict();
            if (indexing!=null) kwargs["indexing"]=ToPython(indexing);
            if (sparse!=null) kwargs["sparse"]=ToPython(sparse);
            if (copy!=null) kwargs["copy"]=ToPython(copy);
            dynamic py = self.InvokeMethod("meshgrid", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        */
        
        /*
        public NDarray<T> meshgrid<T>(T[] x1, x2,…, xn, {‘xy’ indexing = null, bool? sparse = null, bool? copy = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                x1, x2,…, xn,
            });
            var kwargs=new PyDict();
            if (indexing!=null) kwargs["indexing"]=ToPython(indexing);
            if (sparse!=null) kwargs["sparse"]=ToPython(sparse);
            if (copy!=null) kwargs["copy"]=ToPython(copy);
            dynamic py = self.InvokeMethod("meshgrid", args, kwargs);
            return ToCsharp<NDarray<T>>(py);
        }
        */
        
        public NDarray diag(NDarray v, int? k = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                v,
            });
            var kwargs=new PyDict();
            if (k!=null) kwargs["k"]=ToPython(k);
            dynamic py = self.InvokeMethod("diag", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray<T> diag<T>(T[] v, int? k = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                v,
            });
            var kwargs=new PyDict();
            if (k!=null) kwargs["k"]=ToPython(k);
            dynamic py = self.InvokeMethod("diag", args, kwargs);
            return ToCsharp<NDarray<T>>(py);
        }
        
        public NDarray diagflat(NDarray v, int? k = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                v,
            });
            var kwargs=new PyDict();
            if (k!=null) kwargs["k"]=ToPython(k);
            dynamic py = self.InvokeMethod("diagflat", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray<T> diagflat<T>(T[] v, int? k = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                v,
            });
            var kwargs=new PyDict();
            if (k!=null) kwargs["k"]=ToPython(k);
            dynamic py = self.InvokeMethod("diagflat", args, kwargs);
            return ToCsharp<NDarray<T>>(py);
        }
        
        public NDarray tri(int N, int? M = null, int? k = null, Dtype dtype = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                N,
            });
            var kwargs=new PyDict();
            if (M!=null) kwargs["M"]=ToPython(M);
            if (k!=null) kwargs["k"]=ToPython(k);
            if (dtype!=null) kwargs["dtype"]=ToPython(dtype);
            dynamic py = self.InvokeMethod("tri", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray tril(NDarray m, int? k = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                m,
            });
            var kwargs=new PyDict();
            if (k!=null) kwargs["k"]=ToPython(k);
            dynamic py = self.InvokeMethod("tril", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray<T> tril<T>(T[] m, int? k = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                m,
            });
            var kwargs=new PyDict();
            if (k!=null) kwargs["k"]=ToPython(k);
            dynamic py = self.InvokeMethod("tril", args, kwargs);
            return ToCsharp<NDarray<T>>(py);
        }
        
        public NDarray vander(NDarray x, int? N = null, bool? increasing = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                x,
            });
            var kwargs=new PyDict();
            if (N!=null) kwargs["N"]=ToPython(N);
            if (increasing!=null) kwargs["increasing"]=ToPython(increasing);
            dynamic py = self.InvokeMethod("vander", args, kwargs);
            return ToCsharp<NDarray>(py);
        }
        
        public NDarray<T> vander<T>(T[] x, int? N = null, bool? increasing = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                x,
            });
            var kwargs=new PyDict();
            if (N!=null) kwargs["N"]=ToPython(N);
            if (increasing!=null) kwargs["increasing"]=ToPython(increasing);
            dynamic py = self.InvokeMethod("vander", args, kwargs);
            return ToCsharp<NDarray<T>>(py);
        }
        
        /*
        public matrix mat(NDarray data, Dtype dtype)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                data,
                dtype,
            });
            var kwargs=new PyDict();
            dynamic py = self.InvokeMethod("mat", args, kwargs);
            return ToCsharp<matrix>(py);
        }
        */
        
        /*
        public matrix mat<T>(T[] data, Dtype dtype)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                data,
                dtype,
            });
            var kwargs=new PyDict();
            dynamic py = self.InvokeMethod("mat", args, kwargs);
            return ToCsharp<matrix>(py);
        }
        */
        
        /*
        public matrix bmat(string obj, Hashtable ldict = null, Hashtable gdict = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                obj,
            });
            var kwargs=new PyDict();
            if (ldict!=null) kwargs["ldict"]=ToPython(ldict);
            if (gdict!=null) kwargs["gdict"]=ToPython(gdict);
            dynamic py = self.InvokeMethod("bmat", args, kwargs);
            return ToCsharp<matrix>(py);
        }
        */
        
        /*
        public matrix<T> bmat<T>(T[] obj, Hashtable ldict = null, Hashtable gdict = null)
        {
            //auto-generated code, do not change
            var args=ToTuple(new object[]
            {
                obj,
            });
            var kwargs=new PyDict();
            if (ldict!=null) kwargs["ldict"]=ToPython(ldict);
            if (gdict!=null) kwargs["gdict"]=ToPython(gdict);
            dynamic py = self.InvokeMethod("bmat", args, kwargs);
            return ToCsharp<matrix<T>>(py);
        }
        */
        
    }
}
