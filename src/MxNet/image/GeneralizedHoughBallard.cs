using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class GeneralizedHoughBallard : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.GeneralizedHoughBallard;
		public GeneralizedHoughBallard()
		{
			
			__self__ = caller;
		}

		
	}
}