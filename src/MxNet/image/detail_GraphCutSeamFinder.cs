using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class detail_GraphCutSeamFinder : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.detail_GraphCutSeamFinder;
		public detail_GraphCutSeamFinder()
		{
			
			__self__ = caller;
		}

		
	}
}