using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.Gluon.NN
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class ELU : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.ELU;
		public ELU(float alpha)
		{
					Parameters["alpha"] = alpha;

			__self__ = caller;
		}

		
	}
}