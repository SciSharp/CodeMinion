using MxNet.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace MxNet.contrib.text
{
    public class Vocabulary : Base
    {
        public Vocabulary(Counter counter = null, int? most_freq_count = null, int min_freq = 1, string unknown_token = "", string[] reserved_tokens = null)
        {
            __self__ = Instance.mxnet.contrib.text.embedding.FastText;
            Parameters["counter"] = counter;
            Parameters["most_freq_count"] = most_freq_count;
            Parameters["min_freq"] = min_freq;
            Parameters["unknown_token"] = unknown_token;
            Parameters["reserved_tokens"] = reserved_tokens;
            Init();
        }

        public int[] to_indices(string[] tokens)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["tokens"] = tokens;

            return InvokeMethod("to_indices", parameters).As<int[]>();
        }

        public string[] to_tokens(string[] tokens)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["tokens"] = tokens;

            return InvokeMethod("to_indices", parameters).As<string[]>();
        }
    }
}
