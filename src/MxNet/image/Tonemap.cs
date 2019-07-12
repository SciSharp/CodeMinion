using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class Tonemap : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.Tonemap;
		public Tonemap()
		{
			
			__self__ = caller;
		}

		
	}
}