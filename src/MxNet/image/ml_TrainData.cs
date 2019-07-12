using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class ml_TrainData : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.ml_TrainData;
		public ml_TrainData()
		{
			
			__self__ = caller;
		}

		
	}
}