using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.Gluon.NN
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class GlobalAvgPool3D : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.GlobalAvgPool3D;
		public GlobalAvgPool3D(string layout)
		{
					Parameters["layout"] = layout;

			__self__ = caller;
		}

		
	}
}