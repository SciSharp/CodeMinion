using System;
using System.Collections.Generic;
using System.Text;

namespace MxNet.callback
{
    public class LogValidationMetricsCallback : Base
    {
        public LogValidationMetricsCallback()
        {
            __self__ = Instance.mxnet.callback.LogValidationMetricsCallback;
            Init();
        }
    }
}
