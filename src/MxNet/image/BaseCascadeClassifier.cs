using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class BaseCascadeClassifier : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.BaseCascadeClassifier;
		public BaseCascadeClassifier()
		{
			
			__self__ = caller;
		}

		
	}
}