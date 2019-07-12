using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class dnn_Net : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.dnn_Net;
		public dnn_Net()
		{
			
			__self__ = caller;
		}

		
	}
}