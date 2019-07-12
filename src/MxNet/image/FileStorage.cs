using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class FileStorage : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.FileStorage;
		public FileStorage()
		{
			
			__self__ = caller;
		}

		
	}
}