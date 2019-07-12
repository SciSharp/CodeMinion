using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class TonemapMantiuk : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.TonemapMantiuk;
		public TonemapMantiuk()
		{
			
			__self__ = caller;
		}

		
	}
}