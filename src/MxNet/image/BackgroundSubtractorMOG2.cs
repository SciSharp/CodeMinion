using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class BackgroundSubtractorMOG2 : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.BackgroundSubtractorMOG2;
		public BackgroundSubtractorMOG2()
		{
			
			__self__ = caller;
		}

		
	}
}