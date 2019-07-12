using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.Gluon.NN
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class Activation : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.Activation;
		public Activation(string activation)
		{
					Parameters["activation"] = activation;

			__self__ = caller;
		}

		
	}
}