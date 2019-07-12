using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class ml_Boost : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.ml_Boost;
		public ml_Boost()
		{
			
			__self__ = caller;
		}

		
	}
}