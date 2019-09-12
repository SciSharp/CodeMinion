using System;
using System.Collections;
using System.Globalization;
using Regen.DataTypes;

namespace Regen.Builtins {
    public static class Common {
        
        public static void ParseInt(ref object obj) {
            obj = ParseInt(obj);
        }

        public static int ParseInt(object obj) {
            int ret;
            switch (obj) {
                case NumberScalar ns:
                    ret = Convert.ToInt32(ns.Value);
                    break;
                case IConvertible c:
                    ret = c.ToInt32(CultureInfo.InvariantCulture);
                    break;
                case ICollection col:
                    ret = col.Count;
                    break;
                default:
                    throw new NotSupportedException($"Unable to interpret {obj.GetType().Name} as int");
            }

            return ret;
        }
    }
}