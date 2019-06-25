using System;
using System.Collections.Generic;
using System.Text;

namespace Torch.Models.nn
{
    public static partial class torch
    {
        public static partial class nn
        {
            public partial class Module
            {
                /// <summary>
                /// Defines the computation performed at every call.
                /// 
                /// Should be overridden by all subclasses.
                /// 
                /// Note
                /// Although the recipe for forward pass needs to be defined within
                /// this function, one should call the Module instance afterwards
                /// instead of this since the former takes care of running the
                /// registered hooks while the latter silently ignores them.
                /// </summary>
                public virtual void forward(params Tensor[] inputs)
                {
                    throw new NotImplementedException("This function should be overwritten!");
                }
            }
        }
    }
}
