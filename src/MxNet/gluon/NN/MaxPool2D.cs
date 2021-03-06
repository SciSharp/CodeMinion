using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.Gluon.NN
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class MaxPool2D : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.MaxPool2D;
		public MaxPool2D(int[] pool_size,int strides,int[] padding,string layout,bool ceil_mode)
		{
					Parameters["pool_size"] = pool_size;
		Parameters["strides"] = strides;
		Parameters["padding"] = padding;
		Parameters["layout"] = layout;
		Parameters["ceil_mode"] = ceil_mode;

			__self__ = caller;
		}

		
	}
}