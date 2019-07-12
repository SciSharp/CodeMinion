using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class AgastFeatureDetector : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.AgastFeatureDetector;
		public AgastFeatureDetector()
		{
			
			__self__ = caller;
		}

		
	}
}