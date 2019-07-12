using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class ml_ANN_MLP : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.ml_ANN_MLP;
		public ml_ANN_MLP()
		{
			
			__self__ = caller;
		}

		
	}
}