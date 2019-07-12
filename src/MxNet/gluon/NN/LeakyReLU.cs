using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.Gluon.NN
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class LeakyReLU : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.LeakyReLU;
		public LeakyReLU(float alpha)
		{
					Parameters["alpha"] = alpha;

			__self__ = caller;
		}

		
	}
}