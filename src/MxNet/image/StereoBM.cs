using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class StereoBM : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.StereoBM;
		public StereoBM()
		{
			
			__self__ = caller;
		}

		
	}
}