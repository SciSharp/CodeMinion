using System;
using System.Collections.Generic;
using System.Text;
using Python.Runtime;

namespace Numpy
{

    public partial class Dtype : PythonObject
    {
        public Dtype(PyObject pyobj) : base(pyobj)
        {
        }

        public Dtype(Dtype t) : base((PyObject)t.PyObject)
        {
        }

    }
}


//public static dtype GetDtype(this object obj)
//{
//    switch (obj)
//    {
//        case byte o: return dtype.UInt8;
//        case short o: return dtype.Int16;
//        case int o: return dtype.Int32;
//        case long o: return dtype.Int64;
//        case float o: return dtype.Float32;
//        case double o: return dtype.Float64;
//        case byte[] o: return dtype.UInt8;
//        case short[] o: return dtype.Int16;
//        case int[] o: return dtype.Int32;
//        case long[] o: return dtype.Int64;
//        case float[] o: return dtype.Float32;
//        case double[] o: return dtype.Float64;
//        case byte[,] o: return dtype.UInt8;
//        case short[,] o: return dtype.Int16;
//        case int[,] o: return dtype.Int32;
//        case long[,] o: return dtype.Int64;
//        case float[,] o: return dtype.Float32;
//        case double[,] o: return dtype.Float64;
//        case byte[,,] o: return dtype.UInt8;
//        case short[,,] o: return dtype.Int16;
//        case int[,,] o: return dtype.Int32;
//        case long[,,] o: return dtype.Int64;
//        case float[,,] o: return dtype.Float32;
//        case double[,,] o: return dtype.Float64;
//        default: throw new ArgumentException("Can not convert type of given object to dtype: " + obj.GetType());
//    }
//}

//public static dtype ToDtype(this Type t)
//{
//    if (t == typeof(byte)) return dtype.UInt8;
//    if (t == typeof(short)) return dtype.Int16;
//    if (t == typeof(int)) return dtype.Int32;
//    if (t == typeof(long)) return dtype.Int64;
//    if (t == typeof(float)) return dtype.Float32;
//    if (t == typeof(double)) return dtype.Float64;
//    throw new ArgumentException("Can not convert given type to dtype: " + t);
//}
//}
//}
