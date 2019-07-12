using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace MxNet.Collections
{
    public class Counter : Base
    {
        public Counter(PyObject py)
        {
            __self__ = py;
        }

        public Counter(string[] iterable)
        {
            __self__ = Instance.collections.Counter(iterable);
        }

        public Counter(Dictionary<string, int> mapping)
        {
            __self__ = Instance.collections.Counter(mapping);
        }
    }
}
