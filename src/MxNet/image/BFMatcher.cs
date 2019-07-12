using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class BFMatcher : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.BFMatcher;
		public BFMatcher()
		{
			
			__self__ = caller;
		}

		
	}
}