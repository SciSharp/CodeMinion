using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class AlignExposures : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.AlignExposures;
		public AlignExposures()
		{
			
			__self__ = caller;
		}

		
	}
}