using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.Gluon.NN
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class Flatten : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.Flatten;
		public Flatten()
		{
			
			__self__ = caller;
		}

		
	}
}