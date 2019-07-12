using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.Gluon.NN
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class LayerNorm : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.LayerNorm;
		public LayerNorm(int axis,int in_channels)
		{
					Parameters["axis"] = axis;
		Parameters["in_channels"] = in_channels;

			__self__ = caller;
		}

		
	}
}