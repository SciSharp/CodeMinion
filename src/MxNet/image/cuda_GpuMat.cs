using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class cuda_GpuMat : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.cuda_GpuMat;
		public cuda_GpuMat()
		{
			
			__self__ = caller;
		}

		
	}
}