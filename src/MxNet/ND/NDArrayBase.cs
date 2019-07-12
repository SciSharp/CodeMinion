using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.ND
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class NDArrayBase : Base
	{
		private static dynamic caller = Instance.mxnet.ndarray.NDArrayBase;
		public NDArrayBase()
		{
			
			__self__ = caller;
		}

		
	}
}