using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class MSER : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.MSER;
		public MSER()
		{
			
			__self__ = caller;
		}

		
	}
}