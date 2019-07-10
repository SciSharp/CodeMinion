// Code generated by CodeMinion: https://github.com/SciSharp/CodeMinion

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
    public static partial class torch {
        public static partial class nn {
            /// <summary>
            ///	Applies Instance Normalization over a 5D input (a mini-batch of 3D inputs
            ///	with additional channel dimension) as described in the paper
            ///	Instance Normalization: The Missing Ingredient for Fast Stylization .
            ///	
            ///	\[y = \frac{x - \mathrm{E}[x]}{ \sqrt{\mathrm{Var}[x] + \epsilon}} * \gamma + \beta\]
            ///	
            ///	The mean and standard-deviation are calculated per-dimension separately
            ///	for each object in a mini-batch.<br></br>
            ///	 \(\gamma\) and \(\beta\) are learnable parameter vectors
            ///	of size C (where C is the input size) if affine is True.<br></br>
            ///	
            ///	By default, this layer uses instance statistics computed from input data in
            ///	both training and evaluation modes.<br></br>
            ///	
            ///	If track_running_stats is set to True, during training this
            ///	layer keeps running estimates of its computed mean and variance, which are
            ///	then used for normalization during evaluation.<br></br>
            ///	 The running estimates are
            ///	kept with a default momentum of 0.1.
            ///	
            ///	Note
            ///	This momentum argument is different from one used in optimizer
            ///	classes and the conventional notion of momentum.<br></br>
            ///	 Mathematically, the
            ///	update rule for running statistics here is
            ///	\(\hat{x}_\text{new} = (1 - \text{momentum}) \times \hat{x} + \text{momemtum} \times x_t\),
            ///	where \(\hat{x}\) is the estimated statistic and \(x_t\) is the
            ///	new observed value.<br></br>
            ///	
            ///	Note
            ///	InstanceNorm3d and LayerNorm are very similar, but
            ///	have some subtle differences.<br></br>
            ///	 InstanceNorm3d is applied
            ///	on each channel of channeled data like 3D models with RGB color, but
            ///	LayerNorm is usually applied on entire sample and often in NLP
            ///	tasks.<br></br>
            ///	 Additionaly, LayerNorm applies elementwise affine
            ///	transform, while InstanceNorm3d usually don’t apply affine
            ///	transform.
            /// </summary>
            public partial class InstanceNorm3d : Module
            {
                // auto-generated class
                
                public InstanceNorm3d(PyObject pyobj) : base(pyobj) { }
                
                public InstanceNorm3d(Module other) : base(other.PyObject as PyObject) { }
                
                public InstanceNorm3d(int num_features, double eps = 1.0e-5, double momentum = 0.1, bool affine = true, bool track_running_stats = false)
                {
                    //auto-generated code, do not change
                    var nn = self.GetAttr("nn");
                    var __self__=nn;
                    var pyargs=ToTuple(new object[]
                    {
                        num_features,
                    });
                    var kwargs=new PyDict();
                    if (eps!=1.0e-5) kwargs["eps"]=ToPython(eps);
                    if (momentum!=0.1) kwargs["momentum"]=ToPython(momentum);
                    if (affine!=true) kwargs["affine"]=ToPython(affine);
                    if (track_running_stats!=false) kwargs["track_running_stats"]=ToPython(track_running_stats);
                    dynamic py = __self__.InvokeMethod("InstanceNorm3d", pyargs, kwargs);
                    self=py as PyObject;
                }
                
            }
        }
    }
    
}