using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class VideoWriter : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.VideoWriter;
		public VideoWriter()
		{
			
			__self__ = caller;
		}

		
	}
}