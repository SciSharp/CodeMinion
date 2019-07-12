using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class dnn_Layer : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.dnn_Layer;
		public dnn_Layer()
		{
			
			__self__ = caller;
		}

		
	}
}