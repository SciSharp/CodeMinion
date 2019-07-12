using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.ND
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class CachedOp : Base
	{
		private static dynamic caller = Instance.mxnet.ndarray.CachedOp;
		public CachedOp()
		{
			
			__self__ = caller;
		}

		
	}
}