using MxNet.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace MxNet.contrib.text
{
    public class Utils : Base
    {
        static dynamic caller = Instance.mxnet.contrib.utils;

        public static Counter count_tokens_from_str(string source_str, string token_delim= " ", string seq_delim= "\n", bool to_lower= false, Counter counter_to_update= null)
        {

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["source_str"] = source_str;
            parameters["token_delim"] = token_delim;
            parameters["seq_delim"] = seq_delim;
            parameters["to_lower"] = to_lower;
            parameters["counter_to_update"] = counter_to_update;

            return new Counter(InvokeStaticMethod(caller, "get_vecs_by_tokens", parameters));
        }
    }
}
