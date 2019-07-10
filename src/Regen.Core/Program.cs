using System;
using System.IO;
using Regen.Compiler;

namespace Regen {
    public static class Program {
        public static void Main() {
            var code = File.ReadAllText("testfile.cs");

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}