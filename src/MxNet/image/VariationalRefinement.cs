using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class VariationalRefinement : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.VariationalRefinement;
		public VariationalRefinement()
		{
			
			__self__ = caller;
		}

		
	}
}