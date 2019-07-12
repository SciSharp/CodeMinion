using MxNet.logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MxNet.module
{
    public class Module : BaseModule
    {
        public Module(Symbol symbol, string[] data_names = null, string[] label_names = null, Logger logger = null,
                        Context[] context = null, int[] work_load_list = null, string[] fixed_param_names = null,
                        string[] state_names = null, Dictionary<string, Context> group2ctxs = null,
                        Dictionary<string, object> compression_params = null)
        {
            __self__ = Instance.mxnet.module.Module;
            Parameters["symbol"] = symbol;
            Parameters["data_names"] = data_names != null ? data_names : new string[] { "data" };
            Parameters["label_names"] = label_names != null ? label_names : new string[] { "softmax_label" }; ;
            Parameters["logger"] = logger;
            Parameters["context"] = context != null ? context : new Context[] { Context.Default };
            Parameters["work_load_list"] = work_load_list;
            Parameters["fixed_param_names"] = fixed_param_names;
            Parameters["state_names"] = state_names;
            Parameters["group2ctxs"] = group2ctxs;
            Parameters["compression_params"] = compression_params;
            Init();
        }
    }
}
