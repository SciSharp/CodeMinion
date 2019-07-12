using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace MxNet.contrib.text
{
    public class Embedding : Base
    {
        static dynamic caller = Instance.mxnet.contrib.text.embedding;

        public Embedding(PyObject py)
        {
            __self__ = py;
        }

        public static Embedding register(Embedding embedding_cls)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["embedding_cls"] = embedding_cls;

            return new Embedding(InvokeStaticMethod(caller, "register", parameters));
        }

        public static Embedding create(string embedding_name)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["embedding_name"] = embedding_name;

            return new Embedding(InvokeStaticMethod(caller, "create", parameters));
        }

        public static string[] get_pretrained_file_names(string embedding_name)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["embedding_name"] = embedding_name;

            return ((PyObject)InvokeStaticMethod(caller, "get_pretrained_file_names", parameters)).As<string[]>();
        }
    }
}
