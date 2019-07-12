using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class BackgroundSubtractorKNN : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.BackgroundSubtractorKNN;
		public BackgroundSubtractorKNN()
		{
			
			__self__ = caller;
		}

		
	}
}