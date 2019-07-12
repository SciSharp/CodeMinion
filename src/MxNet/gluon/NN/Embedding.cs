using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.Gluon.NN
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class Embedding : Base
	{
		private static dynamic caller = Instance.mxnet.gluon.nn.Embedding;
		public Embedding(int input_dim,int output_dim,DType dtype = null,Initializer weight_initializer = null)
		{
					Parameters["input_dim"] = input_dim;
		Parameters["output_dim"] = output_dim;
		Parameters["dtype"] = dtype;
		Parameters["weight_initializer"] = weight_initializer;

			__self__ = caller;
		}

		
	}
}