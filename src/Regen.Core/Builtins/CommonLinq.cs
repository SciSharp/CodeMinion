using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Regen.DataTypes;

namespace Regen.Builtins
{
    public static class CommonLinq {

        public static Array Except(Array @this, params object[] objs) {
            return Array.Create(@this.Values.Except(objs));
        }

        public static Array Concat(Array @this, params object[] objs) {
            return Array.Create(@this.Values.Concat(objs));
        }

    }
}
