using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class SparseOpticalFlow : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.SparseOpticalFlow;
		public SparseOpticalFlow()
		{
			
			__self__ = caller;
		}

		
	}
}