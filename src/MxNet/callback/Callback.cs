using MxNet.module;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace MxNet.callback
{
    public class Callback : Base
    {
        public Callback(PyObject py)
        {
            __self__ = py;
        }

        static dynamic caller = Instance.mxnet.callback;

        public static Callback module_checkpoint(BaseModule mod, string prefix, int period= 1, bool save_optimizer_states= false)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["mod"] = mod;
            parameters["prefix"] = prefix;
            parameters["period"] = period;
            parameters["save_optimizer_states"] = save_optimizer_states;
            
            return new Callback(InvokeStaticMethod(caller, "module_checkpoint", parameters));
        }

        public static Callback do_checkpoint(string prefix, int period = 1)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["prefix"] = prefix;
            parameters["period"] = period;

            return new Callback(InvokeStaticMethod(caller, "do_checkpoint", parameters));
        }

        public static Callback log_train_metric(int period , bool auto_reset = false)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["period"] = period;
            parameters["auto_reset"] = auto_reset;

            return new Callback(InvokeStaticMethod(caller, "log_train_metric", parameters));
        }
    }
}
