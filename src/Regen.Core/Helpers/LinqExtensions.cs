using System;
using System.Collections.Generic;
using System.Linq;

namespace Regen.Helpers {
    public static class LinqExtensions {
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

        public static string StringJoin(this IEnumerable<string> strs, string seperator = "") {
            return string.Join(seperator, strs);
        }
    }
}