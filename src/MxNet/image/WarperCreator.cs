using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class WarperCreator : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.WarperCreator;
		public WarperCreator()
		{
			
			__self__ = caller;
		}

		
	}
}