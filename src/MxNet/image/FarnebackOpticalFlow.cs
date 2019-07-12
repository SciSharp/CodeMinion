using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class FarnebackOpticalFlow : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.FarnebackOpticalFlow;
		public FarnebackOpticalFlow()
		{
			
			__self__ = caller;
		}

		
	}
}