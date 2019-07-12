using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class ORB : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.ORB;
		public ORB()
		{
			
			__self__ = caller;
		}

		
	}
}