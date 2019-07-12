using System;
using System.Collections.Generic;
using System.Text;
using Python.Runtime;

namespace MxNet.contrib.onnx
{
    public class MX2Onnx : Base
    {
        static dynamic caller = Instance.mxnet.contrib.onnx.mx2onnx.export_model;

        public static string export_model(Symbol sym, Symbol @params, Shape input_shape, DType input_type = null, 
                                        string onnx_file_path = "model.onnx", bool verbose = false)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["sym"] = sym;
            parameters["params"] = @params;
            parameters["input_shape"] = input_shape;
            parameters["input_type"] = input_type;
            parameters["onnx_file_path"] = onnx_file_path;
            parameters["verbose"] = verbose;

            return InvokeStaticMethod(caller, "get_vecs_by_tokens", parameters).ToString();
        }
    }
}
