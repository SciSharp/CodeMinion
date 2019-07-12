using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class BackgroundSubtractor : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.BackgroundSubtractor;
		public BackgroundSubtractor()
		{
			
			__self__ = caller;
		}

		
	}
}