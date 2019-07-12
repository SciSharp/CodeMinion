using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class SimpleBlobDetector : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.SimpleBlobDetector;
		public SimpleBlobDetector()
		{
			
			__self__ = caller;
		}

		
	}
}