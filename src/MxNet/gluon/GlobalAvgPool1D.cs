using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.gluon.nn
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class GlobalAvgPool1D : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.GlobalAvgPool1D;
		public GlobalAvgPool1D(string layout)
		{
					Parameters["layout"] = layout;

			__self__ = caller;
		}

		
	}
}