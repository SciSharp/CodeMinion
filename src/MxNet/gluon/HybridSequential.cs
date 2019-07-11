using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.gluon.nn
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class HybridSequential : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.HybridSequential;
		public HybridSequential()
		{
			
			__self__ = caller;
		}

		
	}
}