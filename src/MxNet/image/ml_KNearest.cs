using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class ml_KNearest : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.ml_KNearest;
		public ml_KNearest()
		{
			
			__self__ = caller;
		}

		
	}
}