using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace MxNet.Helper
{
    public class DictSolver
    {
        public static Dictionary<string, NDArray> ToStrNDArray(PyDict dict)
        {
            Dictionary<string, NDArray> result = new Dictionary<string, NDArray>();
            string[] keys = dict.Keys().As<string[]>();
            foreach (var item in keys)
            {
                result.Add(item, new NDArray(dict[item]));
            }

            return result;
        }

        public static Dictionary<string, Shape> ToStrShape(PyDict dict)
        {
            Dictionary<string, Shape> result = new Dictionary<string, Shape>();
            string[] keys = dict.Keys().As<string[]>();
            foreach (var item in keys)
            {
                result.Add(item, new Shape(dict[item]));
            }

            return result;
        }
    }
}
