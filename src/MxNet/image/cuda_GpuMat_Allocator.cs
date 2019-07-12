using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class cuda_GpuMat_Allocator : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.cuda_GpuMat_Allocator;
		public cuda_GpuMat_Allocator()
		{
			
			__self__ = caller;
		}

		
	}
}