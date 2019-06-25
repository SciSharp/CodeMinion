using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Regen.DataTypes;

namespace Regen.Builtins {
    public static class CommonLinq {
        public static Array Except(IList @this, params object[] objs) {
            return Array.Create(@this.Cast<object>().Except(objs));
        }

        public static Array Concat(IList @this, params object[] objs) {
            return Array.Create(@this.Cast<object>().Concat(objs));
        }
    }
}