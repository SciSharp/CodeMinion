using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.gluon.nn
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class Swish : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.Swish;
		public Swish(float beta)
		{
					Parameters["beta"] = beta;

			__self__ = caller;
		}

		
	}
}