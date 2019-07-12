using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.Gluon.NN
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class GlobalAvgPool2D : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.GlobalAvgPool2D;
		public GlobalAvgPool2D(string layout)
		{
					Parameters["layout"] = layout;

			__self__ = caller;
		}

		
	}
}