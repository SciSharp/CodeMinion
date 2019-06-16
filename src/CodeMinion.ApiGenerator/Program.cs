using System;
using Torch.ApiGenerator;

namespace CodeMinion.ApiGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            ICodeGenerator generator = null;

            switch (args[0].ToLower())
            {
                case "numpy":
                    generator = new NumPy.ApiGenerator();
                    break;
                case "torch":
                    generator = new PyTorch.ApiGenerator();
                    break;
                case "pillow":
                    generator = new Pillow.ApiGenerator();
                    break;
                case "spacy":
                    generator = new SpaCy.ApiGenerator();
                    break;
                default:
                    throw new Exception("Please assign what project you're working on.");
            }

            var result = generator.Generate();

            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}
