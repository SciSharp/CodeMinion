using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Regen.DataTypes;
using Regen.Helpers;

namespace Regen.Builtins {
    public static class CommonLinq {
        public static Array Except(IList @this, params object[] objs) {
            var vals = @this.Cast<Data>().Where(left => objs.Any(right => !Equals(left.Value, right is Data d ? d.Value : right)));
            return Array.Create(vals);
        }

        public static Array Concat(IList @this, params object[] objs) {
            var flattened = objs.Cast<Data>().SelectMany(CollectionSelector);

            return Array.Create(@this.Cast<Data>().Concat(flattened));

            IEnumerable<Data> CollectionSelector(Data data)
            {
                if (data is Array r) return (IList<Data>)r.Values;
                if (data is IList l) return (IList<Data>)l.Cast<Data>().ToList();
                return (IList<Data>)new[] { data };
            }
        }

        public static Array Flatten(params object[] objs) {
            var flattened = objs.Cast<Data>().SelectMany(data => {

                IEnumerable<Data> flat(object obj) {
                    if (obj is ReferenceData reference)
                        return flat(reference.UnpackReference(Compiler.RegenCompiler.CurrentContext));
                    if (obj is Array r)
                        return r.Values.SelectMany(flat).ToArray();
                    if (obj is IList l)
                        return l.Cast<Data>().SelectMany(flat).ToList();
                    return obj.YieldAs<Data>();
                }

                return flat(data);
            }).ToArray();

            return Array.Create(flattened);
        }
    }
}