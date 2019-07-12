using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class Algorithm : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.Algorithm;
		public Algorithm()
		{
			
			__self__ = caller;
		}

		
	}
}