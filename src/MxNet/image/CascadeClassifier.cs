using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class CascadeClassifier : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.CascadeClassifier;
		public CascadeClassifier()
		{
			
			__self__ = caller;
		}

		
	}
}