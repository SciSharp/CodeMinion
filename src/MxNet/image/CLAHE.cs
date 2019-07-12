using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class CLAHE : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.CLAHE;
		public CLAHE()
		{
			
			__self__ = caller;
		}

		
	}
}