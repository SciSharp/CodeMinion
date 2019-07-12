using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class cuda_Event : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.cuda_Event;
		public cuda_Event()
		{
			
			__self__ = caller;
		}

		
	}
}