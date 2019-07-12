using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class KeyPoint : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.KeyPoint;
		public KeyPoint()
		{
			
			__self__ = caller;
		}

		
	}
}