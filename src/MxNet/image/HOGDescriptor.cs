using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class HOGDescriptor : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.HOGDescriptor;
		public HOGDescriptor()
		{
			
			__self__ = caller;
		}

		
	}
}