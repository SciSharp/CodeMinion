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

        public static StringScalar str(params object[] objects) {
            return new StringScalar(string.Join("", objects?.Select(o => o?.ToString() ?? "") ?? new string[] {""}));
        }
        
        //todo add concat(params array)
        //todo add asarray(params)
        //todo add type functions such as 'isarray', 'isnumber', 'isnull' similar to python.

        //todo add a multi-iteration zip that will serve as alternative to nested arrays. consider: [1,2,3] and [4,5,6] togther will result: [(1,4),(1,5),(1,6),  (2,4),(2,5),(2,6) ... so on  - make sure it can be applyed to unlimited amount of arrays.
    }
}