using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class ml_SVM : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.ml_SVM;
		public ml_SVM()
		{
			
			__self__ = caller;
		}

		
	}
}