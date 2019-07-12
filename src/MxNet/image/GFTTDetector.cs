using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class GFTTDetector : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.GFTTDetector;
		public GFTTDetector()
		{
			
			__self__ = caller;
		}

		
	}
}