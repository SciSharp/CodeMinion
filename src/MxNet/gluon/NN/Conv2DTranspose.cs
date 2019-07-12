using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.Gluon.NN
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class Conv2DTranspose : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.Conv2DTranspose;
		public Conv2DTranspose(int channels,int[] strides,int[] padding,int[] dilation,int groups,string layout,int in_channels,string activation,bool use_bias,StringOrInitializer weight_initializer = null,StringOrInitializer bias_initializer = null)
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