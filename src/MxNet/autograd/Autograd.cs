using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace MxNet.autograd
{
    public class Autograd : Base
    {
        static dynamic caller = Instance.mxnet.autograd;

        public static bool set_recording(bool is_recording)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["is_recording"] = is_recording;
            return (bool)InvokeStaticMethod(caller, "set_recording", parameters);
        }

        public static bool set_training(bool train_mode)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["train_mode"] = train_mode;
            return (bool)InvokeStaticMethod(caller, "set_training", parameters);
        }

        public static bool is_training()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            return (bool)InvokeStaticMethod(caller, "is_training", parameters);
        }

        public static object record(bool train_mode = true)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["train_mode"] = train_mode;
            return InvokeStaticMethod(caller, "record", parameters);
        }

        public static object pause(bool train_mode = true)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["train_mode"] = train_mode;
            return InvokeStaticMethod(caller, "pause", parameters);
        }

        public static object train_mode()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            return InvokeStaticMethod(caller, "train_mode", parameters);
        }

        public static object predict_mode()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            return (bool)InvokeStaticMethod(caller, "predict_mode", parameters);
        }

        public static void mark_variables(NDArray[] variables, NDArray[] gradients, string[] grad_reqs)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["variables"] = variables;
            parameters["gradients"] = gradients;
            parameters["grad_reqs"] = grad_reqs;
            InvokeStaticMethod(caller, "mark_variables", parameters);
        }

        public static NDArray[] grad(NDArray[] heads, NDArray[] variables, NDArray[] head_grads, bool retain_graph, bool create_graph, bool? train_mode = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["heads"] = heads;
            parameters["variables"] = variables;
            parameters["head_grads"] = head_grads;
            parameters["retain_graph"] = retain_graph;
            parameters["create_graph"] = create_graph;
            parameters["train_mode"] = train_mode;
            PyObject pyobj = (PyObject)InvokeStaticMethod(caller, "grad", parameters);
            List<NDArray> result = new List<NDArray>();

            foreach (PyObject item in pyobj)
            {
                result.Add(new NDArray(item));
            }

            return result.ToArray();
        }

        public static Symbol get_symbol(NDArray x)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            return new Symbol(InvokeStaticMethod(caller, "predict_mode", parameters));
        }
    }
}
