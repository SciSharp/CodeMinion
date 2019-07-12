using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.Gluon.NN
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class Lambda : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.Lambda;
		public Lambda(object function)
		{
					Parameters["function"] = function;

			__self__ = caller;
		}

		
	}
}