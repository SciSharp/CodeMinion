using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.Gluon.NN
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class SELU : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.SELU;
		public SELU()
		{
			
			__self__ = caller;
		}

		
	}
}