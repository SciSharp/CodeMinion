using CodeMinion.Core.Helpers;

namespace Numpy.ApiGenerator
{
    public static class SpecialGenerators
    {

        public static void InitNumpyGenerator(CodeWriter s)
        {
            s.Out("installer.InstallWheel(typeof(NumPy).Assembly, \"numpy-1.16.3-cp37-cp37m-win_amd64.whl\").Wait();");
        }
    }
}
