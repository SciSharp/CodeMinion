using MxNet.callback;
using System;
using System.Collections.Generic;
using System.Text;

namespace MxNet.contrib.text
{
    public class GloVe : Base
    {
        public GloVe(string pretrained_file_name = "glove.840B.300d.txt", string embedding_root = "./embeddings", Callback init_unknown_vec = null, 
                        Vocabulary vocabulary = null)
        {
            __self__ = Instance.mxnet.contrib.text.embedding.GloVe;
            Parameters["pretrained_file_name"] = pretrained_file_name;
            Parameters["embedding_root"] = embedding_root;
            Parameters["init_unknown_vec"] = init_unknown_vec;
            Parameters["vocabulary"] = vocabulary;
            Init();
        }

        public NDArray get_vecs_by_tokens(string[] tokens, bool lower_case_backup = false)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["tokens"] = tokens;
            parameters["lower_case_backup"] = lower_case_backup;

            return new NDArray(InvokeMethod("get_vecs_by_tokens", parameters));
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

        public void update_token_vectors(string[] tokens, NDArray new_vectors)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["tokens"] = tokens;
            parameters["new_vectors"] = new_vectors;

            InvokeMethod("update_token_vectors", parameters).As<string[]>();
        }
    }
}
