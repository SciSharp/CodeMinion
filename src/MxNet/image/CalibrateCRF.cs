using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class CalibrateCRF : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.CalibrateCRF;
		public CalibrateCRF()
		{
			
			__self__ = caller;
		}

		
	}
}