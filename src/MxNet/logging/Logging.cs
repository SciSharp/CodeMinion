using System;
using System.Collections.Generic;
using System.Text;

namespace MxNet.logging
{
    public class Logging : Base
    {
        public Logging()
        {
            __self__ = Instance.logging;
            Init();
        }

        public const int CRITICAL = 50;
        public const int FATAL = 50;
        public const int ERROR = 40;
        public const int WARNING = 30;
        public const int WARN = 30;
        public const int INFO = 20;
        public const int DEBUG = 10;
        public const int NOTSET = 0;

        public Logger getLogger(string name = "")
        {
            return new Logger(Instance.logging.getLogger(name));
        }
    }
}
