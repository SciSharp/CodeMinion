using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class BOWTrainer : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.BOWTrainer;
		public BOWTrainer()
		{
			
			__self__ = caller;
		}

		
	}
}