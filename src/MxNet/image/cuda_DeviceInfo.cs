using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class cuda_DeviceInfo : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.cuda_DeviceInfo;
		public cuda_DeviceInfo()
		{
			
			__self__ = caller;
		}

		
	}
}