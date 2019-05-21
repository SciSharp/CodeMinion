using System;

namespace PillowSharp.ApiGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var pillow_gen = new PillowApiGenerator();
            pillow_gen.Generate();
        }
    }
}
