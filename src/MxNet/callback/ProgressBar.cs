using System;
using System.Collections.Generic;
using System.Text;

namespace MxNet.callback
{
    public class ProgressBar : Base
    {
        public ProgressBar(int total, int length = 80)
        {
            __self__ = Instance.mxnet.callback.ProgressBar;
            Parameters["total"] = total;
            Parameters["length"] = length;
            Init();
        }
    }
}
