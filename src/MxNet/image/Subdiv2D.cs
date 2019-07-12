using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class Subdiv2D : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.Subdiv2D;
		public Subdiv2D()
		{
			
			__self__ = caller;
		}

		
	}
}