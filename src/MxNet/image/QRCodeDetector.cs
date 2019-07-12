using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class QRCodeDetector : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.QRCodeDetector;
		public QRCodeDetector()
		{
			
			__self__ = caller;
		}

		
	}
}