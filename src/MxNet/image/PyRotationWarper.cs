using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class PyRotationWarper : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.PyRotationWarper;
		public PyRotationWarper()
		{
			
			__self__ = caller;
		}

		
	}
}