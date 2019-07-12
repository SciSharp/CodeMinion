using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class KAZE : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.KAZE;
		public KAZE()
		{
			
			__self__ = caller;
		}

		
	}
}