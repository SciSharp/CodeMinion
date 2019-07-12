using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class detail_FeaturesMatcher : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.detail_FeaturesMatcher;
		public detail_FeaturesMatcher()
		{
			
			__self__ = caller;
		}

		
	}
}