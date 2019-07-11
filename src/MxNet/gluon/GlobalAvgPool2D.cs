using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.gluon.nn
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