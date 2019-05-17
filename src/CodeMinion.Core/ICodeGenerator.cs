using System;
using System.Collections.Generic;
using System.Text;

namespace Torch.CodeGenerator
{
    public interface ICodeGenerator
    {
        string Generate();

        Dictionary<string, string> LoadDocs();
    }
}
