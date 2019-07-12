using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class KalmanFilter : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.KalmanFilter;
		public KalmanFilter()
		{
			
			__self__ = caller;
		}

		
	}
}