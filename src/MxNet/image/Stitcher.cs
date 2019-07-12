using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class Stitcher : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.Stitcher;
		public Stitcher()
		{
			
			__self__ = caller;
		}

		
	}
}