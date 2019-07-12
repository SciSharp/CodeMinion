using MxNet.gluon;
using MxNet.Helper;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace MxNet.contrib.onnx
{
    public class Onnx2MX : Base
    {
        static dynamic caller = Instance.mxnet.contrib.onnx.onnx2mx;

        public static (Symbol, Dictionary<string, NDArray>, Dictionary<string, NDArray>) import_model(string model_file)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["model_file"] = model_file;

            PyObject py = InvokeStaticMethod(caller.import_model, "import_model", parameters);
            PyTuple tuple = new PyTuple(py);
            Symbol sym = new Symbol(tuple[0]);

            PyDict argParams = new PyDict(tuple[1]);
            PyDict auxParams = new PyDict(tuple[2]);

            return (sym, DictSolver.ToStrNDArray(argParams), DictSolver.ToStrNDArray(auxParams));
        }

        public static Dictionary<string, Shape> get_model_metadata(string model_file)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["model_file"] = model_file;

            PyObject py = InvokeStaticMethod(caller.import_model, "import_model", parameters);
            PyDict dict = new PyDict(py);

            return DictSolver.ToStrShape(dict);
        }

        public static SymbolBlock import_to_gluon(string model_file, Context ctx)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["model_file"] = model_file;
            parameters["ctx"] = ctx;

            PyObject py = InvokeStaticMethod(caller.import_to_gluonr, "import_model", parameters);

            return new SymbolBlock(py);
        }
    }
}
