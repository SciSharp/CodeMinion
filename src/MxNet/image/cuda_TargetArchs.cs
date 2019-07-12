using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class cuda_TargetArchs : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.cuda_TargetArchs;
		public cuda_TargetArchs()
		{
			
			__self__ = caller;
		}

		
	}
}