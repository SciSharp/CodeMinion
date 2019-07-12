using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class GeneralizedHoughGuil : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.GeneralizedHoughGuil;
		public GeneralizedHoughGuil()
		{
			
			__self__ = caller;
		}

		
	}
}