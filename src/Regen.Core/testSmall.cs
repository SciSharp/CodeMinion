using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regen {


#if _REGEN
        %import namespace.Type as potato
        %foreach operators%
        public static dynamic operator #1 (OperatorsOverloading lhs, int rhs) {
            dynamic left = lhs.Value;
            dynamic right = rhs;
            return left #1 right;
        }
        %
#else


#endif


}