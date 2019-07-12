using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class MergeExposures : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.MergeExposures;
		public MergeExposures()
		{
			
			__self__ = caller;
		}

		
	}
}