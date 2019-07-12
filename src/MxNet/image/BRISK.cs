using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class BRISK : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.BRISK;
		public BRISK()
		{
			
			__self__ = caller;
		}

		
	}
}