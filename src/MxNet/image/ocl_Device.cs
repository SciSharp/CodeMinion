using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class ocl_Device : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.ocl_Device;
		public ocl_Device()
		{
			
			__self__ = caller;
		}

		
	}
}