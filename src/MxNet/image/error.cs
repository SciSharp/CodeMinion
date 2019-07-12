using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class error : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.error;
		public error()
		{
			
			__self__ = caller;
		}

		
	}
}