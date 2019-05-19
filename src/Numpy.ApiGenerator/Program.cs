using System;

namespace Torch.ApiGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var numpy_gen = new NumpyApiGenerator();
            numpy_gen.Generate();
        }
    }
}
