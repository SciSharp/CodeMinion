using Python.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Numpy;
using Numpy.Models;


namespace Torch
{
    // note: this file contains manually overridden implementations
    public partial class PyTorch
    {

        public Tensor tensor(NDarray data, Dtype dtype = null, Device device = null, bool? requires_grad = null, bool? pin_memory = null)
        {
            //auto-generated code, do not change
            var __self__ = self;
            var pyargs = ToTuple(new object[]
            {
                data.PyObject,
            });
            var kwargs = new PyDict();
            if (dtype != null) kwargs["dtype"] = ToPython(dtype);
            //if (layout != null) kwargs["layout"] = ToPython(layout);
            if (device != null) kwargs["device"] = ToPython(device);
            if (requires_grad != null) kwargs["requires_grad"] = ToPython(requires_grad);
            if (pin_memory != null) kwargs["pin_memory"] = ToPython(pin_memory);
            dynamic py = __self__.InvokeMethod("empty", pyargs, kwargs);
            return ToCsharp<Tensor>(py);
        }

        public Tensor<T> tensor<T>(T[] data, Dtype dtype = null, Device device = null, bool? requires_grad = null, bool? pin_memory = null)
        {
            // note: this implementation works only for device CPU
            // todo: implement for GPU
            var type = data.GetDtype();
            if (dtype!=null && type!=dtype)
                throw new NotImplementedException("Type of the array is different from specified dtype. Data conversion is not supported (yet)");
            var tensor = torch.empty(new Shape(data.Length), dtype: type, device: device,
                requires_grad: requires_grad, pin_memory: pin_memory);
            var storage = tensor.PyObject.storage();
            long ptr = storage.data_ptr();
            switch ((object)data) {
                case byte[] a: Marshal.Copy(a, 0, new IntPtr(ptr), a.Length); break;
                case short[] a: Marshal.Copy(a, 0, new IntPtr(ptr), a.Length); break;
                case int[] a: Marshal.Copy(a, 0, new IntPtr(ptr), a.Length); break;
                case long[] a: Marshal.Copy(a, 0, new IntPtr(ptr), a.Length); break;
                case float[] a: Marshal.Copy(a, 0, new IntPtr(ptr), a.Length); break;
                case double[] a: Marshal.Copy(a, 0, new IntPtr(ptr), a.Length); break;
            }
            return new Tensor<T>( tensor);
        }

        //public Tensor<T> tensor<T>(T[,] data, dtype? dtype = null, device? device = null, bool? requires_grad = null, bool? pin_memory = null)
        //{
        //    // note: this implementation works only for device CPU
        //    // todo: implement for GPU
        //    var type = data.GetDtype();
        //    if (dtype != null && type != dtype)
        //        throw new NotImplementedException("Type of the array is different from specified dtype. Data conversion is not supported (yet)");
        //    var tensor = empty(new Shape(data.Length), dtype: type, device: device,
        //        requires_grad: requires_grad, pin_memory: pin_memory);
        //    var storage = tensor.PyObject.storage();
        //    long ptr = storage.data_ptr();
        //    switch ((object)data)
        //    {
        //        case byte[,] a: Marshal.Copy(a, 0, new IntPtr(ptr), a.Length); break;
        //        case short[,] a: Marshal.Copy(a, 0, new IntPtr(ptr), a.Length); break;
        //        case int[,] a: Marshal.Copy(a, 0, new IntPtr(ptr), a.Length); break;
        //        case long[,] a: Marshal.Copy(a, 0, new IntPtr(ptr), a.Length); break;
        //        case float[,] a: Marshal.Copy(a, 0, new IntPtr(ptr), a.Length); break;
        //        case double[,] a: Marshal.Copy(a, 0, new IntPtr(ptr), a.Length); break;
        //    }
        //    return new Tensor<T>(tensor);
        //}

    }
}
