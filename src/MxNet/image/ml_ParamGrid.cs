using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class ml_ParamGrid : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.ml_ParamGrid;
		public ml_ParamGrid()
		{
			
			__self__ = caller;
		}

		
	}
}