using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class TonemapDrago : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.TonemapDrago;
		public TonemapDrago()
		{
			
			__self__ = caller;
		}

		
	}
}