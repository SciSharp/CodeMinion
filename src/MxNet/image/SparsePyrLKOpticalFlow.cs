using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class SparsePyrLKOpticalFlow : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.SparsePyrLKOpticalFlow;
		public SparsePyrLKOpticalFlow()
		{
			
			__self__ = caller;
		}

		
	}
}