using System;
using System.Collections.Generic;
using System.Text;

namespace MxNet
{
    public class Context : Base
    {
        public static Context Default = new Context(DeviceType.CPU, 0);

        public Context(DeviceType device_type, int device_id = 0)
        {
            __self__ = Instance.mxnet.Context;
            Parameters["device_type"] = (int)device_type;
            Parameters["device_id"] = device_id;
            Init();
        }

        public DeviceType device_type
        {
            get
            {
                return __self__.GetAttr("device_type").As<int>();
            }
        }

        public override string ToString()
        {
            return __self__.GetAttr("__str__").ToString();
        }
    }

    public enum DeviceType
    {
        CPU = 1,
        GPU = 2,
        CPUPinned = 3,
        CPUShared = 5
    }
}
