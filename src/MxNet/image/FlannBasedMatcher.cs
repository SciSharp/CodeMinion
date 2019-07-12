using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class FlannBasedMatcher : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.FlannBasedMatcher;
		public FlannBasedMatcher()
		{
			
			__self__ = caller;
		}

		
	}
}