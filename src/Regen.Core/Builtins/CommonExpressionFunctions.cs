using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Regen.Compiler;
using Regen.Compiler.Helpers;
using Regen.DataTypes;
using Array = Regen.DataTypes.Array;

namespace Regen.Builtins {
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class CommonExpressionFunctions {
        public static int len(ICollection arr) {
            return arr.Count;
        }        
        
        public static int len(Array arr) {
            return arr.Count;
        }        

        public static Array range(int length) {
            return new Array(Enumerable.Range(0, length).Select(r => new NumberScalar(r)).Cast<Data>().ToList());
        }

        public static Array range(int startFrom, int length) {
            return new Array(Enumerable.Range(startFrom, length).Select(r => new NumberScalar(r)).Cast<Data>().ToList());
        }

        /// <summary>
        ///     Zips all items 
        /// </summary>
        /// <returns></returns>
        public static PackedArguments zipmax(params object[] objects) {
            return new PackedArguments(objects.Cast<IList>().ToArray());
        }

        /// <summary>
        ///     Zips all items 
        /// </summary>
        /// <returns></returns>
        public static PackedArguments ziplongest(params object[] objects) {
            return zipmax(objects);
        }

        public static StringScalar str(params object[] objects) {
            return new StringScalar(string.Join("", objects?.Select(o => o?.ToString() ?? "") ?? new string[] {""}));
        }

        public static bool isarray(object obj) {
            return obj is IList; //Array implements IList.
        }

        public static bool isstr(object obj) {
            return obj is StringScalar || obj is string;
        }

        public static bool isnumber(object obj) {
            if (obj == null) return false;

            if (obj is decimal || obj is NumberScalar) return true;
            var type = obj.GetType();
            return type.IsPrimitive;
        }

        public static bool isnull(object obj) {
            return obj == null || obj is NullScalar;
        }

        public static Array asarray(params object[] objs) {
            return Array.CreateParams(objs);
        }

        //todo add concat(params array)
        //todo add asarray(params)
        //todo add type functions such as 'isarray', 'isnumber', 'isnull' similar to python.

        //todo add a multi-iteration zip that will serve as alternative to nested arrays. consider: [1,2,3] and [4,5,6] togther will result: [(1,4),(1,5),(1,6),  (2,4),(2,5),(2,6) ... so on  - make sure it can be applyed to unlimited amount of arrays.
    }
}