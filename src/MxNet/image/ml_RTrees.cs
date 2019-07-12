using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class ml_RTrees : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.ml_RTrees;
		public ml_RTrees()
		{
			
			__self__ = caller;
		}

		
	}
}