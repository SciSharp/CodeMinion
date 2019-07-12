using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class ml_LogisticRegression : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.ml_LogisticRegression;
		public ml_LogisticRegression()
		{
			
			__self__ = caller;
		}

		
	}
}