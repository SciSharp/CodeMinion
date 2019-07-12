using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class ml_EM : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.ml_EM;
		public ml_EM()
		{
			
			__self__ = caller;
		}

		
	}
}