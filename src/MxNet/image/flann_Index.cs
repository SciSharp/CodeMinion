using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class flann_Index : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.flann_Index;
		public flann_Index()
		{
			
			__self__ = caller;
		}

		
	}
}