using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class ml_StatModel : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.ml_StatModel;
		public ml_StatModel()
		{
			
			__self__ = caller;
		}

		
	}
}