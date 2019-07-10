using System;
using Torch.ApiGenerator;

namespace CodeMinion.ApiGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            ICodeGenerator generator = null;
            if (args.Length==0)
                throw new Exception("Please set the command line parameter to the project you want to generate.");
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
                case "keras":
                    generator = new Keras.ApiGenerator();
                    break;
                case "mxnet":
                    generator = new MxNet.ApiGenerator();
                    break;
                default:
                    throw new Exception("Please assign what project you're working on.");
            }

            var result = generator.Generate();

            Console.WriteLine(result);
            //Console.ReadKey();
        }
    }
}
