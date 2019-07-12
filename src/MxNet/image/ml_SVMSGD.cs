using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class ml_SVMSGD : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.ml_SVMSGD;
		public ml_SVMSGD()
		{
			
			__self__ = caller;
		}

		
	}
}