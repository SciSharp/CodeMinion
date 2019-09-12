using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Regen.Compiler;
using Regen.Compiler.Helpers;
using Regen.DataTypes;
using Regen.Parser.Expressions;
using Array = Regen.DataTypes.Array;

namespace Regen.Builtins {
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public partial class CommonExpressionFunctions {
        public static int len(ICollection arr) {
            return arr.Count;
        }

        public static int len(Array arr) {
            return arr.Count;
        }

        public static Array range(object length) {
            return new Array(Enumerable.Range(0, toindex(length)).Select(r => new NumberScalar(r)).Cast<Data>().ToList());
        }

        public static Array range(object startFrom, object length) {
            return new Array(Enumerable.Range(toindex(startFrom), toindex(length)).Select(r => new NumberScalar(r)).Cast<Data>().ToList());
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

        public static StringScalar join(object seperator, params object[] objects) {
            if (objects != null && objects.Length == 1 && objects[0] is IList l) {
                return @join(seperator, l);
            }

            return new StringScalar(string.Join((string) str(seperator).Value, objects?.Select(o => o?.ToString() ?? "") ?? new string[] {""}));
        }

        public static StringScalar join(object seperator, IList objects) {
            return new StringScalar(string.Join((string) str(seperator).Value, objects?.Cast<object>().Select(o => o?.ToString() ?? "") ?? new string[] {""}));
        }

        public static NumberScalar number(object obj) {
            if (obj is NumberScalar s)
                return s;
            if (obj is Data d) {
                var v = d.EmitExpressive().Trim('\"');
                if (v.Contains("."))
                    return new NumberScalar(int.Parse(v));
                return new NumberScalar(double.Parse(v));
            }

            {
                var v = obj?.ToString().Trim('\"');
                if (v == null)
                    return new NumberScalar(0);

                if (v.Contains("."))
                    return new NumberScalar(int.Parse(v));
                return new NumberScalar(double.Parse(v));
            }
        }

        public static int toindex(object obj) {
            return toint(obj);
        }

        public static int toint(object obj) {
            return number(obj).Cast<int>();
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

        /// <summary>
        ///     Passing [1,2] , [3,4] will result in [1,1,2,2] [3,4,3,4]
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="excludeEquals"></param>
        /// <returns></returns>
        public static PackedArguments forevery(IList first, IList second, bool excludeEquals) {
            var retFirst = new List<object>();
            var retSecond = new List<object>();
            foreach (var f in first) {
                foreach (var s in second) {
                    if (excludeEquals && Equals(f, s)) {
                        continue;
                    }

                    retFirst.Add(f);
                    retSecond.Add(s);
                }
            }

            return new PackedArguments(Array.Create(retFirst), Array.Create(retSecond));
        }

        /// <summary>
        ///     Passing [1,2] , [3,4] will result in [1,1,2,2] [3,4,3,4]
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="excludeEquals"></param>
        /// <returns></returns>
        public static PackedArguments forevery(IList first, IList second, IList third, bool excludeEquals) {
            var retFirst = new List<object>();
            var retSecond = new List<object>();
            var retThird = new List<object>();
            foreach (var f in first) {
                foreach (var s in second) {
                    foreach (var t in third) {
                        if (excludeEquals && (Equals(f, s) || Equals(f, t) || Equals(s, t))) {
                            continue;
                        }

                        retFirst.Add(f);
                        retSecond.Add(s);
                        retThird.Add(t);
                    }
                }
            }

            return new PackedArguments(Array.Create(retFirst), Array.Create(retSecond), Array.Create(retThird));
        }

        public static StringScalar Repeat(object exprObj, int repeats, object exprBetween, object exprBeforeFirst, object exprAfterFirst) {
            return Repeat(exprObj, repeats, exprBetween, exprBeforeFirst, exprAfterFirst, Data.Null, Data.Null);
        }

        public static StringScalar Repeat(object exprObj, int repeats, object exprBetween) {
            return Repeat(exprObj, repeats, exprBetween, Data.Null, Data.Null, Data.Null, Data.Null);
        }

        public static StringScalar Repeat(object exprObj, int repeats) {
            return Repeat(exprObj, repeats, Data.Null, Data.Null, Data.Null, Data.Null, Data.Null);
        }

        public static StringScalar Repeat(object exprObj, NumberScalar repeats, object exprBetween, object exprBeforeFirst, object exprAfterFirst, object exprBeforeLast, object exprAfterLast) {
            return Repeat(exprObj, (object) repeats.Value, exprBetween, exprBeforeFirst, exprAfterFirst, exprBeforeLast, exprAfterLast);
        }

        public static StringScalar Repeat(object exprObj, NumberScalar repeats, object exprBetween, object exprBeforeFirst, object exprAfterFirst) {
            return Repeat(exprObj, repeats, exprBetween, exprBeforeFirst, exprAfterFirst, Data.Null, Data.Null);
        }

        public static StringScalar Repeat(object exprObj, NumberScalar repeats, object exprBetween) {
            return Repeat(exprObj, repeats, exprBetween, Data.Null, Data.Null, Data.Null, Data.Null);
        }

        public static StringScalar Repeat(object exprObj, NumberScalar repeats) {
            return Repeat(exprObj, repeats, Data.Null, Data.Null, Data.Null, Data.Null, Data.Null);
        }

        public static StringScalar Repeat(object exprObj, object repeats_, object exprBetween, object exprBeforeFirst, object exprAfterFirst, object exprBeforeLast, object exprAfterLast) {
            var repeats = int.Parse(repeats_ is Data d ? d.EmitExpressive() : repeats_.ToString());
            if (repeats <= 0)
                return new StringScalar("");

            Console.WriteLine($"{exprObj.GetType()}");
            var expr = exprObj.ToString();
            var bf = exprBeforeFirst?.ToString() ?? "";
            var af = exprAfterFirst?.ToString() ?? "";
            var bl = exprBeforeLast?.ToString() ?? "";
            var al = exprAfterLast?.ToString() ?? "";
            var between = exprBetween?.ToString() ?? "";

            var compiler = RegenCompiler.CurrentCompiler;
            var original_n = compiler.Context.Variables.ContainsKey("n") ? compiler.Context.Variables["n"] : null;
            try {
                string word = expr;

                //parse if ^ is found at start
                bool compile = word.StartsWith("^");
                if (compile)
                    word = word.Substring(1);

                //trim escaped
                if (word.StartsWith("\\^"))
                    word = word.TrimStart('\\');

                bool compileBetween = between.StartsWith("^");
                if (compileBetween)
                    between = between.Substring(1);

                //trim escaped
                if (between.StartsWith("\\^"))
                    between = between.TrimStart('\\');

                if (repeats == 1) {
                    compiler.Context.Variables["n"] = 0;
                    return new StringScalar($"{bf}{(compile ? compiler.EvaluateString(word) ?? word : word)}{al}");
                }

                var sb = new StringBuilder();
                compiler.Context.Variables["n"] = 0;
                sb.Append($"{bf}{(compile ? compiler.EvaluateString(word) ?? word : word)}{af}");
                sb.Append(compileBetween ? compiler.EvaluateString(between) ?? between : between);
                for (int i = 1; i < repeats - 1; i++) {
                    compiler.Context.Variables["n"] = i;
                    sb.Append(compile ? compiler.EvaluateString(word) ?? word : word);
                    sb.Append(compileBetween ? compiler.EvaluateString(between) ?? between : between);
                }

                compiler.Context.Variables["n"] = repeats - 1;
                sb.Append($"{bl}{(compile ? compiler.EvaluateString(word) ?? word : word)}{al}");

                return new StringScalar(sb.ToString());
            } finally {
                if (original_n != null)
                    compiler.Context.Variables["n"] = original_n;
                else
                    compiler.Context.Variables.Remove("n");
            }
        }
    }
}