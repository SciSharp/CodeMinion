using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class StereoMatcher : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.StereoMatcher;
		public StereoMatcher()
		{
			
			__self__ = caller;
		}

		
	}
}