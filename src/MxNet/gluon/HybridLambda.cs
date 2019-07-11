using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.gluon.nn
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class HybridLambda : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.HybridLambda;
		public HybridLambda(object function)
		{
					Parameters["function"] = function;

			__self__ = caller;
		}

		
	}
}