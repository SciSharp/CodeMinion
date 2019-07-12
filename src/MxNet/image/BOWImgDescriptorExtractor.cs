using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class BOWImgDescriptorExtractor : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.BOWImgDescriptorExtractor;
		public BOWImgDescriptorExtractor()
		{
			
			__self__ = caller;
		}

		
	}
}