using System.Collections.Generic;
using System.Linq;

namespace Regen.Helpers.Collections {
    /// <summary>
    ///     A <see cref="List{T}"/> with operator capabilities.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OList<T> : List<T> {
        /// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.List`1" /> class that is empty and has the default initial capacity.</summary>
        public OList() { }

        /// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.List`1" /> class that is empty and has the specified initial capacity.</summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than 0. </exception>
        public OList(int capacity) : base(capacity) { }

        /// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.List`1" /> class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.</summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="collection" /> is <see langword="null" />.</exception>
        public OList(IEnumerable<T> collection) : base(collection) { }

        public static OList<T> operator +(OList<T> list, T t) {
            list.Add(t);
            return list;
        }

        public static OList<T> operator +(OList<T> list, IEnumerable<T> t) {
            list.AddRange(t);
            return list;
        }

        public static OList<T> operator -(OList<T> list, T t) {
            list.Remove(t);
            return list;
        }

        public static OList<T> operator -(OList<T> list, IEnumerable<T> t) {
            var todelete = t.ToList();
            list.RemoveAll(v => todelete.Contains(v));
            return list;
        }

        public static OList<T> operator &(OList<T> list, IEnumerable<T> t) {
            var other = t.ToList();
            var intersect = list.Intersect(other).ToArray();
            list.RemoveAll(v => !intersect.Contains(v));
            return list;
        }

        public static OList<T> operator ^(OList<T> list, IEnumerable<T> t) {
            var other = t.ToList();
            var intersect = list.Except(other).ToArray();
            list.RemoveAll(v => !intersect.Contains(v));
            return list;
        }
    }
}