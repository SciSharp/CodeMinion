using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.Gluon.NN
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class Dropout : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.Dropout;
		public Dropout(float rate,int[] axes)
		{
					Parameters["rate"] = rate;
		Parameters["axes"] = axes;

			__self__ = caller;
		}

		
	}
}