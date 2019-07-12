using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class DenseOpticalFlow : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.DenseOpticalFlow;
		public DenseOpticalFlow()
		{
			
			__self__ = caller;
		}

		
	}
}