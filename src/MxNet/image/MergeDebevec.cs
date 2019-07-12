using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class MergeDebevec : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.MergeDebevec;
		public MergeDebevec()
		{
			
			__self__ = caller;
		}

		
	}
}