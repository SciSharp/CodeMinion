using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MxNet.Gluon.NN
{
    /// <summary>
    /// [COMMENTS]
    /// </summary>
	public class Dense : Base
    {
        private static dynamic caller = Instance.mxnet.gluon.nn.Dense;
        public Dense(int units, string activation, bool use_bias, DType dtype = null, StringOrInitializer weight_initializer = null, int in_units = 0)
        {
            Parameters["units"] = units;
            Parameters["activation"] = activation;
            Parameters["use_bias"] = use_bias;
            Parameters["dtype"] = dtype;
            Parameters["weight_initializer"] = weight_initializer;
            Parameters["in_units"] = in_units;

            __self__ = caller;
        }


    }
}