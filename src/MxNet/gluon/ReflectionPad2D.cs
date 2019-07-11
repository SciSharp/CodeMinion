using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.gluon.nn
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class ReflectionPad2D : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.ReflectionPad2D;
		public ReflectionPad2D(int padding)
		{
					Parameters["padding"] = padding;

			__self__ = caller;
		}

		
	}
}