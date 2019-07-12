using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.image
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class LineSegmentDetector : Base
	{
		private static dynamic caller = Instance.mxnet.cv2.LineSegmentDetector;
		public LineSegmentDetector()
		{
			
			__self__ = caller;
		}

		
	}
}