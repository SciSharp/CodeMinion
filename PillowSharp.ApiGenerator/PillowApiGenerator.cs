using CodeMinion.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Torch.ApiGenerator;

namespace PillowSharp.ApiGenerator
{
    class PillowApiGenerator : ICodeGenerator
    {
        private CodeGenerator _generator;

        public string Generate()
        {
            throw new NotImplementedException();
        }

        string BaseUrl = "https://pillow.readthedocs.io/en/stable/reference/";

        public Dictionary<string, string> LoadDocs()
        {
            throw new NotImplementedException();
        }
    }
}
