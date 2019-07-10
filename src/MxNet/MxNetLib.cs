using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using static Python.Runtime.Py;

namespace MxNet
{
    public class MxNetLib : IDisposable
    {
        public static MxNetLib Instance => _instance.Value;

        private static Lazy<MxNetLib> _instance = new Lazy<MxNetLib>(() =>
        {
            var instance = new MxNetLib();
            instance.mxnet = InstallAndImport("mxnet");
            return instance;
        }
        );

        private static PyObject InstallAndImport(string module)
        {
            if(!PythonEngine.IsInitialized)
                PythonEngine.Initialize();
            var mod = Py.Import(module);
            return mod;
        }

        public dynamic mxnet = null;

        private bool IsInitialized => mxnet != null;

        internal MxNetLib() { }

        public void Dispose()
        {
            mxnet?.Dispose();
            PythonEngine.Shutdown();
        }

        internal static PyObject ToPython(object obj)
        {
            if (obj == null) return Runtime.GetPyNone();
            switch (obj)
            {
                // basic types
                case int o: return new PyInt(o);
                case float o: return new PyFloat(o);
                case double o: return new PyFloat(o);
                case string o: return new PyString(o);
                case bool o:
                    if (o)
                        return new PyObject(Runtime.PyTrue);
                    else
                        return new PyObject(Runtime.PyFalse);

                // sequence types
                case Array o: return ToList(o);
                // special types from 'ToPythonConversions'
                case Shape o: return ToTuple(o.Dimensions);
                case PyObject o: return o;
                case Base o: return o.ToPython();
                default: throw new NotImplementedException($"Type is not yet supported: { obj.GetType().Name}. Add it to 'ToPythonConversions'");
            }
        }

        protected static PyTuple ToTuple(Array input)
        {
            var array = new PyObject[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                array[i] = ToPython(input.GetValue(i));
            }

            return new PyTuple(array);
        }

        protected static PyList ToList(Array input)
        {
            var array = new PyObject[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                array[i] = ToPython(input.GetValue(i));
            }

            return new PyList(array);
        }

    }
}
