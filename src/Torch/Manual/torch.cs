using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Python.Runtime;
using Numpy;
using Numpy.Models;

namespace Torch
{
    // note: this file contains manually overridden API functions
    public static partial class torch
    {

        public static Tensor tensor(NDarray data, Dtype dtype = null, Device device = null, bool? requires_grad = null, bool? pin_memory = null)
            => PyTorch.Instance.tensor(data, dtype: dtype, device: device, requires_grad: requires_grad, pin_memory: pin_memory);

        public static Tensor<T> tensor<T>(T[] data, Dtype dtype = null, Device device = null, bool? requires_grad = null, bool? pin_memory = null)
            => PyTorch.Instance.tensor(data, dtype: dtype, device: device, requires_grad: requires_grad, pin_memory: pin_memory);

        // layouts
        public static Layout strided => new Layout(PyTorch.Instance.self.GetAttr("strided"));
        public static Layout sparse_coo => new Layout(PyTorch.Instance.self.GetAttr("sparse_coo"));

        // dtypes
        public static Dtype float32 => new Dtype(PyTorch.Instance.self.GetAttr("float32"));
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
        public static Dtype FloatTensor => new Dtype(PyTorch.Instance.self.GetAttr("FloatTensor"));
        public static Dtype DoubleTensor => new Dtype(PyTorch.Instance.self.GetAttr("DoubleTensor"));
        public static Dtype IntTensor => new Dtype(PyTorch.Instance.self.GetAttr("IntTensor"));
        public static Dtype LongTensor => new Dtype(PyTorch.Instance.self.GetAttr("LongTensor"));
        public static Dtype ByteTensor => new Dtype(PyTorch.Instance.self.GetAttr("ByteTensor"));

        /// <summary>
        /// A torch.device is an object representing the device on which a torch.Tensor is or will be allocated.
        /// 
        /// The torch.device contains a device type ('cpu' or 'cuda') and optional device ordinal for the device type.
        /// If the device ordinal is not present, this represents the current device for the device type; e.g.a torch.
        /// Tensor constructed with device 'cuda' is equivalent to 'cuda:X' where X is the result of
        /// torch.cuda.current_device().
        /// 
        /// A torch.Tensor’s device can be accessed via the Tensor.device property.
        /// A torch.device can be constructed via a string or via a string and device ordinal
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Device device(string name, int? index=null)
        {
            if (index==null)
                return new Device(PyTorch.Instance.self.InvokeMethod("device", PyTorch.Instance.ToTuple(new object[]{name})));
            return new Device(PyTorch.Instance.self.InvokeMethod("device", PyTorch.Instance.ToTuple(new object[] { name, index.Value })));
        }

        /// <summary>
        /// Return a cuda device
        /// </summary>
        public static Device device(int index)
        {
            return new Device(PyTorch.Instance.self.InvokeMethod("device", PyTorch.Instance.ToTuple(new object[] { index})));
        }

        /// <summary>
        /// Returns a tensor of random numbers drawn from separate normal distributions whose mean
        /// and standard deviation are given.
        /// 
        /// The shapes of mean and std don’t need to match, but the total number of elements in each tensor need to be the same.
        /// 
        /// NOTE: When the shapes do not match, the shape of mean is used as the shape for the returned output tensor
        /// </summary>
        /// <param name="mean">The mean is a tensor with the mean of each output element’s normal distribution</param>
        /// <param name="std">The std is a tensor with the standard deviation of each output element’s normal distribution</param>
        /// <param name="out">The output tensor</param>
        /// <returns></returns>
        public static Tensor normal(Tensor mean, Tensor std, Tensor @out = null)
            => PyTorch.Instance.normal(mean, std, @out: @out);

        /// <summary>
        /// Returns a tensor of random numbers drawn from separate normal distributions whose mean
        /// and standard deviation are given.
        /// </summary>
        /// <param name="mean">The mean for all distributions (default: 0 if null)</param>
        /// <param name="std">The std is a tensor with the standard deviation of each output element’s normal distribution</param>
        /// <param name="out">The output tensor</param>
        /// <returns></returns>
        public static Tensor normal(float? mean, Tensor std, Tensor @out = null)
            => PyTorch.Instance.normal(mean: mean, std: std, @out: @out);

        /// <summary>
        /// Returns a tensor of random numbers drawn from separate normal distributions whose mean
        /// and standard deviation are given.
        /// </summary>
        /// <param name="mean">The mean is a tensor with the mean of each output element’s normal distribution</param>
        /// <param name="std">The standard deviation for all distributions</param>
        /// <param name="out">The output tensor</param>
        /// <returns></returns>
        public static Tensor normal(Tensor mean, float std = 1.0f, Tensor @out = null)
            => PyTorch.Instance.normal(mean, std: std, @out: @out);

        /// <summary>
        /// Adds the scalar value to each element of the input input and returns a new resulting tensor.
        /// 
        /// out=input+value
        /// If input is of type FloatTensor or DoubleTensor, value must be a real number, otherwise it should be an integer.
        /// </summary>
        /// <param name="input">the input tensor</param>
        /// <param name="@value">the number to be added to each element of input</param>
        /// <param name="out"> the output tensor</param>
        public static void @add(Tensor input, object @value, Tensor @out = null)
            => PyTorch.Instance.@add(input, @value, @out: @out);

        /// <summary>
        /// Each element of the tensor other is multiplied by the scalar value and added to each element of the tensor input.
        /// The resulting tensor is returned. The shapes of input and other must be broadcastable.
        /// 
        /// out=input+value×other
        /// If other is of type FloatTensor or DoubleTensor, value must be a real number, otherwise it should be an integer.
        /// </summary>
        /// <param name="input">the first input tensor</param>
        /// <param name="value">the scalar multiplier for other</param>
        /// <param name="other">the second input tensor</param>
        /// <param name="out">the output tensor</param>
        public static void @add(Tensor input, object @value, Tensor other, Tensor @out = null)
            => PyTorch.Instance.@add(input, @value: @value, other: other, @out: @out);

        /// <summary>
        /// Each element of the tensor other added to each element of the tensor input.
        /// The resulting tensor is returned. The shapes of input and other must be broadcastable.
        /// 
        /// out=input+other
        /// </summary>
        /// <param name="input">the first input tensor</param>
        /// <param name="other">the second input tensor</param>
        /// <param name="out">the output tensor</param>
        public static void @add(Tensor input, Tensor other, Tensor @out = null)
            => PyTorch.Instance.@add(input, @value: 1, other: other, @out: @out);

        /// <summary>
        /// Returns a tensor filled with random numbers from a uniform distribution
        /// on the interval \([0, 1)\)
        /// 
        /// The shape of the tensor is defined by the variable argument sizes.
        /// </summary>
        /// <param name="sizes">
        /// a sequence of integers defining the shape of the output tensor.
        /// Can be a variable number of arguments or a collection like a list or tuple.
        /// </param>
        public static Tensor rand(params int[] sizes)
            => PyTorch.Instance.rand(new Shape(sizes));

        /// <summary>
        /// Returns a tensor filled with random numbers from a normal distribution
        /// with mean 0 and variance 1 (also called the standard normal
        /// distribution).
        /// 
        /// \[\text{out}_{i} \sim \mathcal{N}(0, 1)
        /// 
        /// \]
        /// 
        /// The shape of the tensor is defined by the variable argument sizes.
        /// </summary>
        /// <param name="sizes">
        /// a sequence of integers defining the shape of the output tensor.
        /// Can be a variable number of arguments or a collection like a list or tuple.
        /// </param>
        public static Tensor randn(params int[] sizes)
            => PyTorch.Instance.randn(new Shape(sizes));

        /// <summary>
        /// Returns a tensor filled with the scalar value 0, with the shape defined
        /// by the variable argument sizes.
        /// </summary>
        /// <param name="sizes">
        /// a sequence of integers defining the shape of the output tensor.
        /// Can be a variable number of arguments or a collection like a list or tuple.
        /// </param>
        public static Tensor zeros(params int[] sizes)
            => PyTorch.Instance.zeros(new Shape(sizes));

        /// <summary>
        /// Returns a tensor filled with the scalar value 1, with the shape defined
        /// by the variable argument sizes.
        /// </summary>
        /// <param name="sizes">
        /// a sequence of integers defining the shape of the output tensor.
        /// Can be a variable number of arguments or a collection like a list or tuple.
        /// </param>
        public static Tensor ones(params int[] sizes)
            => PyTorch.Instance.ones(new Shape(sizes));

    }
}

