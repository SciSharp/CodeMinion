using System;
using System.Collections.Generic;
using System.Text;
using CodeMinion.Core.Attributes;

namespace CodeMinion.ApiGenerator.PyTorch.Templates
{
    [PartialClassTemplate("Tensor<T>", "GetData<T>")]
    public class TensorMembersGenerator
    {
    }
}
