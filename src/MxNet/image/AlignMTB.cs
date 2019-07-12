using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class AlignMTB : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.AlignMTB;
		public AlignMTB()
		{
			
			__self__ = caller;
		}

		
	}
}