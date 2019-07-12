using System;
using System.Collections.Generic;
using System.Text;

namespace MxNet.callback
{
    public class Speedometer : Base
    {
        public Speedometer(int batch_size, int frequent = 50, bool auto_reset = true)
        {
            __self__ = Instance.mxnet.callback.Speedometer;
            Parameters["batch_size"] = batch_size;
            Parameters["frequent"] = frequent;
            Parameters["auto_reset"] = auto_reset;
            Init();
        }
    }
}
