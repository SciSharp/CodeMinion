using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Regen.DataTypes;
using Regen.Helpers;
using Array = Regen.DataTypes.Array;

namespace Regen.Builtins {
    public static class CommonLinq {
        public static Array RepeatElement(object obj, object times) {
            int repeats = Common.ParseInt(times);

            if (obj is ReferenceData r)
                obj = r.Value;

            if (obj is IList l)
                return Array.Create(Repeat(l.Cast<object>().Select(Data.Create), repeats));

            return Array.Create(Enumerable.Repeat(obj is Data d ? d : Data.Create(obj), repeats));
        }

        private static IEnumerable<T> Repeat<T>(IEnumerable<T> @in, int times) {
            var data = @in.ToArray();
            for (var i = 0; i < times; i++) {
                foreach (var val in data) {
                    yield return val;
                }
            }
        }

        public static Array Except(IList @this, params object[] objs) {
            if (objs == null || objs.Length == 0)
                return Array.Create(@this);

            var vals = @this.Cast<Data>().Where(left => objs.Any(right => !Equals(left.Value, right is Data d ? d.Value : right)));
            return Array.Create(vals);
        }

        public static Array Take(IList @this, object count) {
            int intTake = Common.ParseInt(count);

            if (@this == null || @this.Count == 0)
                return Array.Create(@this);

            var vals = @this.Cast<Data>().Take(intTake);
            return Array.Create(vals);
        }

        public static Array Skip(IList @this, object count) {
            int intSkip = Common.ParseInt(count);

            if (@this == null || @this.Count == 0)
                return Array.Create(@this);

            var vals = @this.Cast<Data>().Skip(intSkip);
            return Array.Create(vals);
        }

        public static Array SkipTake(IList @this, object skip, object take) {
            int intSkip = Common.ParseInt(skip);
            int intTake = Common.ParseInt(take);

            if (@this == null || @this.Count == 0)
                return Array.Create(@this);

            var vals = @this.Cast<Data>().Skip(intSkip).Take(intTake);
            return Array.Create(vals);
        }

        public static Array Concat(IList @this, params object[] objs) {
            if (objs == null || objs.Length == 0)
                return Array.Create(@this);

            var flattened = objs.Cast<Data>().SelectMany(CollectionSelector);

            return Array.Create(@this.Cast<Data>().Concat(flattened));

            IEnumerable<Data> CollectionSelector(Data data) {
                if (data is Array r) return (IList<Data>) r.Values;
                if (data is IList l) return (IList<Data>) l.Cast<Data>().ToList();
                return (IList<Data>) new[] {data};
            }
        }

        public static Array Flatten(params object[] objs) {
            if (objs == null || objs.Length == 0)
                return Array.Create(new object[0]);

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