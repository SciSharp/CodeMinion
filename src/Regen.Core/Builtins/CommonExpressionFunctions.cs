using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Regen.Compiler;
using Regen.DataTypes;
using Array = Regen.DataTypes.Array;

namespace Regen.Builtins {
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class CommonExpressionFunctions {
        public static int len(ICollection arr) {
            return arr.Count;
        }

        public static Array range(int count) {
            return new Array(Enumerable.Range(0, count).Select(r => new NumberScalar(r)).Cast<Scalar>().ToList());
        }

        public static Array range(int @from, int count) {
            return new Array(Enumerable.Range(@from, count).Select(r => new NumberScalar(r)).Cast<Scalar>().ToList());
        }

        /// <summary>
        ///     Zips all items 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static PackedArguments zipmax(params object[] objects) {
            return objects.Concat(new object[] {new ForeachConfig() {Length = ForeachInstance.StackLength.LargestIndex}}).ToArray();
        }

        /// <summary>
        ///     Zips all items 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static PackedArguments ziplongest(params object[] objects) {
            return zipmax(objects);
        }
    }
}