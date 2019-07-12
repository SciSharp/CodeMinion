using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class MergeMertens : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.MergeMertens;
		public MergeMertens()
		{
			
			__self__ = caller;
		}

		
	}
}