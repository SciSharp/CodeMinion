using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.gluon.nn
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class HybridBlock : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.HybridBlock;
		public HybridBlock()
		{
			
			__self__ = caller;
		}

		
	}
}