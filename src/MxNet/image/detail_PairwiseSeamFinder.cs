using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class detail_PairwiseSeamFinder : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.detail_PairwiseSeamFinder;
		public detail_PairwiseSeamFinder()
		{
			
			__self__ = caller;
		}

		
	}
}