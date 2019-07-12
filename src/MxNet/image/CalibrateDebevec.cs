using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class CalibrateDebevec : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.CalibrateDebevec;
		public CalibrateDebevec()
		{
			
			__self__ = caller;
		}

		
	}
}