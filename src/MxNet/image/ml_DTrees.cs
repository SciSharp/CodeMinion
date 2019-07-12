using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class ml_DTrees : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.ml_DTrees;
		public ml_DTrees()
		{
			
			__self__ = caller;
		}

		
	}
}