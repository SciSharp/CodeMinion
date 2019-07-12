using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class detail_NoSeamFinder : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.detail_NoSeamFinder;
		public detail_NoSeamFinder()
		{
			
			__self__ = caller;
		}

		
	}
}