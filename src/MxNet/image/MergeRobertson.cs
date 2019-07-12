using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class MergeRobertson : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.MergeRobertson;
		public MergeRobertson()
		{
			
			__self__ = caller;
		}

		
	}
}