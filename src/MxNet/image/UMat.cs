using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class UMat : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.UMat;
		public UMat()
		{
			
			__self__ = caller;
		}

		
	}
}