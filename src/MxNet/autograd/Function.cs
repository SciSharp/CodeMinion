using System;
using System.Collections.Generic;
using System.Text;

namespace MxNet.autograd
{
    public abstract class Function
    {
        public abstract NDArray Forward(NDArray x);

        public abstract NDArray Backward(NDArray dy);
    }
}
