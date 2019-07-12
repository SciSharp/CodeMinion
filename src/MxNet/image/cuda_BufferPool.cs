using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class cuda_BufferPool : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.cuda_BufferPool;
		public cuda_BufferPool()
		{
			
			__self__ = caller;
		}

		
	}
}