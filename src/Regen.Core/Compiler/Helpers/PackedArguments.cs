using System;
using System.Collections;

namespace Regen.Compiler.Helpers {
    /// <summary>
    ///     Represents packed arguments that are to be unpacked when value is to be used.
    /// </summary>
    public class PackedArguments : IList {
        public IList[] Objects { get; set; }

        public PackedArguments(params IList[] objects) {
            Objects = objects;
        }


        public static implicit operator PackedArguments(IList[] objs) {
            return new PackedArguments(objs);
        }

        #region IList Impl

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return Objects.GetEnumerator();
        }

        /// <summary>Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection" />. The <see cref="T:System.Array" /> must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="array" /> is multidimensional.-or- The number of elements in the source <see cref="T:System.Collections.ICollection" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.-or-The type of the source <see cref="T:System.Collections.ICollection" /> cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
        void ICollection.CopyTo(Array array, int index) {
            Objects.CopyTo(array, index);
        }

        /// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.ICollection" />.</summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.ICollection" />.</returns>
        int ICollection.Count => Objects.Length;

        /// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</summary>
        /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</returns>
        object ICollection.SyncRoot => Objects.SyncRoot;

        /// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).</summary>
        /// <returns>
        /// <see langword="true" /> if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, <see langword="false" />.</returns>
        bool ICollection.IsSynchronized => Objects.IsSynchronized;

        /// <summary>Adds an item to the <see cref="T:System.Collections.IList" />.</summary>
        /// <param name="value">The object to add to the <see cref="T:System.Collections.IList" />. </param>
        /// <returns>The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection.</returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList" /> is read-only.-or- The <see cref="T:System.Collections.IList" /> has a fixed size. </exception>
        int IList.Add(object value) {
            return ((IList) Objects).Add(value);
        }

        /// <summary>Determines whether the <see cref="T:System.Collections.IList" /> contains a specific value.</summary>
        /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList" />. </param>
        /// <returns>
        /// <see langword="true" /> if the <see cref="T:System.Object" /> is found in the <see cref="T:System.Collections.IList" />; otherwise, <see langword="false" />.</returns>
        bool IList.Contains(object value) {
            return ((IList) Objects).Contains(value);
        }

        /// <summary>Removes all items from the <see cref="T:System.Collections.IList" />.</summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList" /> is read-only. </exception>
        void IList.Clear() {
            throw new NotSupportedException();
        }

        /// <summary>Determines the index of a specific item in the <see cref="T:System.Collections.IList" />.</summary>
        /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList" />. </param>
        /// <returns>The index of <paramref name="value" /> if found in the list; otherwise, -1.</returns>
        int IList.IndexOf(object value) {
            return Array.IndexOf(Objects, value);
        }

        /// <summary>Inserts an item to the <see cref="T:System.Collections.IList" /> at the specified index.</summary>
        /// <param name="index">The zero-based index at which <paramref name="value" /> should be inserted. </param>
        /// <param name="value">The object to insert into the <see cref="T:System.Collections.IList" />. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.IList" />. </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList" /> is read-only.-or- The <see cref="T:System.Collections.IList" /> has a fixed size. </exception>
        /// <exception cref="T:System.NullReferenceException">
        /// <paramref name="value" /> is null reference in the <see cref="T:System.Collections.IList" />.</exception>
        void IList.Insert(int index, object value) {
            throw new NotSupportedException();
        }

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList" />.</summary>
        /// <param name="value">The object to remove from the <see cref="T:System.Collections.IList" />. </param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList" /> is read-only.-or- The <see cref="T:System.Collections.IList" /> has a fixed size. </exception>
        void IList.Remove(object value) {
            throw new NotSupportedException();
        }

        /// <summary>Removes the <see cref="T:System.Collections.IList" /> item at the specified index.</summary>
        /// <param name="index">The zero-based index of the item to remove. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.IList" />. </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList" /> is read-only.-or- The <see cref="T:System.Collections.IList" /> has a fixed size. </exception>
        void IList.RemoveAt(int index) {
            throw new NotSupportedException();
        }

        /// <summary>Gets or sets the element at the specified index.</summary>
        /// <param name="index">The zero-based index of the element to get or set. </param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.IList" />. </exception>
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.IList" /> is read-only. </exception>
        object IList.this[int index] {
            get => Objects[index];
            set => Objects[index] = (IList) value;
        }

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.IList" /> is read-only.</summary>
        /// <returns>
        /// <see langword="true" /> if the <see cref="T:System.Collections.IList" /> is read-only; otherwise, <see langword="false" />.</returns>
        bool IList.IsReadOnly => Objects.IsReadOnly;

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.IList" /> has a fixed size.</summary>
        /// <returns>
        /// <see langword="true" /> if the <see cref="T:System.Collections.IList" /> has a fixed size; otherwise, <see langword="false" />.</returns>
        bool IList.IsFixedSize => Objects.IsFixedSize;

        #endregion
    }
}