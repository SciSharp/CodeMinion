using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class GeneralizedHough : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.GeneralizedHough;
		public GeneralizedHough()
		{
			
			__self__ = caller;
		}

		
	}
}