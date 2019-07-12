using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.Gluon.NN
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class InstanceNorm : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.InstanceNorm;
		public InstanceNorm(int axis,int in_channels)
		{
					Parameters["axis"] = axis;
		Parameters["in_channels"] = in_channels;

			__self__ = caller;
		}

		
	}
}