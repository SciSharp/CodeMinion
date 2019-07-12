using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class Feature2D : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.Feature2D;
		public Feature2D()
		{
			
			__self__ = caller;
		}

		
	}
}