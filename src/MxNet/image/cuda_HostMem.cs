using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class cuda_HostMem : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.cuda_HostMem;
		public cuda_HostMem()
		{
			
			__self__ = caller;
		}

		
	}
}