using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class FileNode : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.FileNode;
		public FileNode()
		{
			
			__self__ = caller;
		}

		
	}
}