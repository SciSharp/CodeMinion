using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.Gluon.NN
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class GlobalMaxPool2D : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.GlobalMaxPool2D;
		public GlobalMaxPool2D(string layout)
		{
					Parameters["layout"] = layout;

			__self__ = caller;
		}

		
	}
}