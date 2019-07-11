using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.gluon.nn
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class Conv3D : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.Conv3D;
		public Conv3D(int channels,int[] strides,int[] padding,int[] dilation,int groups,string layout,int in_channels,string activation,bool use_bias,StringOrInitializer weight_initializer = null,StringOrInitializer bias_initializer = null)
		{
					Parameters["channels"] = channels;
		Parameters["strides"] = strides;
		Parameters["padding"] = padding;
		Parameters["dilation"] = dilation;
		Parameters["groups"] = groups;
		Parameters["layout"] = layout;
		Parameters["in_channels"] = in_channels;
		Parameters["activation"] = activation;
		Parameters["use_bias"] = use_bias;
		Parameters["weight_initializer"] = weight_initializer;
		Parameters["bias_initializer"] = bias_initializer;

			__self__ = caller;
		}

		
	}
}