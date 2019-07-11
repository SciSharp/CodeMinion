using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.gluon.nn
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class PReLU : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.PReLU;
		public PReLU(Initializer alpha_initializer = null)
		{
					Parameters["alpha_initializer"] = alpha_initializer;

			__self__ = caller;
		}

		
	}
}