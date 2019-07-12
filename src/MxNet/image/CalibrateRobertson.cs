using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class CalibrateRobertson : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.CalibrateRobertson;
		public CalibrateRobertson()
		{
			
			__self__ = caller;
		}

		
	}
}