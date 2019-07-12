using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.Gluon.NN
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class GlobalMaxPool3D : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.GlobalMaxPool3D;
		public GlobalMaxPool3D(string layout)
		{
					Parameters["layout"] = layout;

			__self__ = caller;
		}

		
	}
}