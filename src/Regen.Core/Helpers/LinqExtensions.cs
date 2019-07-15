using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Regen.Helpers {
    public static class LinqExtensions {

        public static IEnumerable<T> Yield<T>(this T obj) {
            yield return obj;
        }

        public static IEnumerable<T> YieldAs<T>(this object obj) {
            yield return (T) obj;
        }

        /// <summary>
        ///     Does exactly what <see cref="System.Linq.TakeWhile()"/> does but also takes the last unmatching item.
        /// </summary>
        public static IEnumerable<T> TakeWhileIncluding<T>(this IEnumerable<T> data, Func<T, bool> predicate) {
            foreach (var item in data) {
                yield return item;
                if (!predicate(item))
                    break;
            }
        }

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int count) {
            var buffer = new List<T>();
            int pos = 0;

            foreach (var item in source) {
                if (buffer.Count < count) {
                    // phase 1
                    buffer.Add(item);
                } else {
                    // phase 2
                    buffer[pos] = item;
                    pos = (pos + 1) % count;
                }
            }

            for (int i = 0; i < buffer.Count; i++) {
                yield return buffer[pos];
                pos = (pos + 1) % count;
            }
        }

        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int count) {
            var buffer = new List<T>();
            int pos = 0;

            foreach (var item in source) {
                if (buffer.Count < count) {
                    // phase 1
                    buffer.Add(item);
                } else {
                    // phase 2
                    yield return buffer[pos];
                    buffer[pos] = item;
                    pos = (pos + 1) % count;
                }
            }
        }
        [DebuggerStepThrough]
        public static string StringJoin(this IEnumerable<string> strs, string seperator = "") {
            if (strs == null)
                return "";
            return string.Join(seperator, strs);
        }
    }
}