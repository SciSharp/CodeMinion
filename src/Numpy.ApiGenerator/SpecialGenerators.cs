using CodeMinion.Core.Helpers;

namespace Numpy.ApiGenerator
{
    public static class SpecialGenerators
    {

        public static void InitNumpyGenerator(CodeWriter s)
        {
            s.Out("installer.InstallWheel(typeof(NumPy).Assembly, \"numpy-1.16.3-cp37-cp37m-win_amd64.whl\").Wait();");
        }

        public static void ArrayToNDarrayConversion(CodeWriter s)
        {
            s.Out("case Array a:");
            s.Out("if (typeof(T)==typeof(NDarray)) return (T)(object)ConvertArrayToNDarray(a);");
            s.Out("break;");
        }

        public static void ConvertArrayToNDarray(CodeWriter s)
        {
            s.Out("protected NDarray ConvertArrayToNDarray(Array a)", () =>
            {
                s.Out("switch(a)", () =>
                {
                    s.Out("case bool[] arr: return np.array(arr);");
                    s.Out("default: throw new NotImplementedException($\"Type {a.GetType()} not supported yet in ConvertArrayToNDarray.\");");
                });
            });
        }
        
    }
}
