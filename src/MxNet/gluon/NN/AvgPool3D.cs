using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.Gluon.NN
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class AvgPool3D : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.AvgPool3D;
		public AvgPool3D(int[] pool_size,int strides,int[] padding,string layout,bool ceil_mode,bool count_include_pad)
		{
					Parameters["pool_size"] = pool_size;
		Parameters["strides"] = strides;
		Parameters["padding"] = padding;
		Parameters["layout"] = layout;
		Parameters["ceil_mode"] = ceil_mode;
		Parameters["count_include_pad"] = count_include_pad;

			__self__ = caller;
		}

		
	}
}