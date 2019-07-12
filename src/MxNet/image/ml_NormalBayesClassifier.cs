using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class ml_NormalBayesClassifier : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.ml_NormalBayesClassifier;
		public ml_NormalBayesClassifier()
		{
			
			__self__ = caller;
		}

		
	}
}