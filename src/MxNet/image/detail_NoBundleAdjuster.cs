using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class detail_NoBundleAdjuster : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.detail_NoBundleAdjuster;
		public detail_NoBundleAdjuster()
		{
			
			__self__ = caller;
		}

		
	}
}