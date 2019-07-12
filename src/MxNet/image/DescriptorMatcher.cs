using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class DescriptorMatcher : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.DescriptorMatcher;
		public DescriptorMatcher()
		{
			
			__self__ = caller;
		}

		
	}
}