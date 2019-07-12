using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class VideoCapture : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.VideoCapture;
		public VideoCapture()
		{
			
			__self__ = caller;
		}

		
	}
}