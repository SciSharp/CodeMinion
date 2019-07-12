using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class FastFeatureDetector : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.FastFeatureDetector;
		public FastFeatureDetector()
		{
			
			__self__ = caller;
		}

		
	}
}