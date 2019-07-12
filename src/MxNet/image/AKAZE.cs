using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class AKAZE : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.AKAZE;
		public AKAZE()
		{
			
			__self__ = caller;
		}

		
	}
}