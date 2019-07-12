using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class TonemapReinhard : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.TonemapReinhard;
		public TonemapReinhard()
		{
			
			__self__ = caller;
		}

		
	}
}