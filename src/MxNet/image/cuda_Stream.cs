using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class cuda_Stream : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.cuda_Stream;
		public cuda_Stream()
		{
			
			__self__ = caller;
		}

		
	}
}