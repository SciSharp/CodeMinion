using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Regen.DataTypes {
    [DebuggerDisplay("Array: {this}")]
    public class Array : Data, IList<Scalar>, ICollection<Scalar>, ICollection, IList, IEnumerable<Scalar> {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public List<Scalar> Values { get; set; }

        public Array() {
            Values = new List<Scalar>(0);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Array(List<Scalar> values) {
            Values = values;
        }

        public override object Value {
            get => Values;
            set => Values = (List<Scalar>) value;
        }

        public object this[int index] {
            get => Values[index].Value;
            set => Values[index] = Scalar.Create(value);
        }

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.IList" /> is read-only.</summary>
        /// <returns>
        /// <see langword="true" /> if the <see cref="T:System.Collections.IList" /> is read-only; otherwise, <see langword="false" />.</returns>
        bool IList.IsReadOnly => ((IList) Values).IsReadOnly;

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.IList" /> has a fixed size.</summary>
        /// <returns>
        /// <see langword="true" /> if the <see cref="T:System.Collections.IList" /> has a fixed size; otherwise, <see langword="false" />.</returns>
        bool IList.IsFixedSize => ((IList) Values).IsFixedSize;

        public override string Emit() {
            return $"[{string.Join("|", Values.Select(v => v.Emit()))}]";
        }

        /// <summary>
        ///     Emit the <see cref="Data.Value"/> for expression evaluation purposes.
        /// </summary>
        /// <returns></returns>
        public override string EmitExpressive() {
            return $"(new object[]{{{string.Join(",", Values.Select(v => v.EmitExpressive()))}}})";
        }

        public string Emit(int index) {
            return Values[index].Emit();
        }

        /// <summary>
        ///     Creates an array with given values that are wrapped using <see cref="Scalar.Create"/>.
        /// </summary>
        /// <param name="objs">Objects that are supported by <see cref="Scalar.Create"/>.</param>
        public static Array Create(params object[] objs) {
            if (objs == null || objs.Length == 0)
                return new Array();
            return new Array(objs.Select(Scalar.Create).ToList());
        }

        /// <summary>
        ///     Creates an array with given values that are wrapped using <see cref="Scalar.Create"/>.
        /// </summary>
        /// <param name="objs">Objects that are supported by <see cref="Scalar.Create"/>.</param>
        public static Array Create(IEnumerable<object> objs) {
            if (objs == null)
                return new Array();
            var objs_ = objs.ToList();
            if (objs_.Count == 0)
                return new Array();

            return new Array(objs_.Select(Scalar.Create).ToList());
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<Scalar> GetEnumerator() {
            return Values.GetEnumerator();
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() {
            return Emit();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable) Values).GetEnumerator();
        }

        #region Delegating

        /// <summary>Adds an object to the end of the <see cref="T:System.Collections.Generic.List`1" />.</summary>
        /// <param name="item">The object to be added to the end of the <see cref="T:System.Collections.Generic.List`1" />. The value can be <see langword="null" /> for reference types.</param>
        public void Add(Scalar item) {
            Values.Add(item);
        }

        /// <summary>Adds the elements of the specified collection to the end of the <see cref="T:System.Collections.Generic.List`1" />.</summary>
        /// <param name="collection">The collection whose elements should be added to the end of the <see cref="T:System.Collections.Generic.List`1" />. The collection itself cannot be <see langword="null" />, but it can contain elements that are <see langword="null" />, if type <paramref name="T" /> is a reference type.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="collection" /> is <see langword="null" />.</exception>
        public void AddRange(IEnumerable<Scalar> collection) {
            Values.AddRange(collection);
        }

        /// <summary>Searches the entire sorted <see cref="T:System.Collections.Generic.List`1" /> for an element using the specified comparer and returns the zero-based index of the element.</summary>
        /// <param name="item">The object to locate. The value can be <see langword="null" /> for reference types.</param>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IComparer`1" /> implementation to use when comparing elements.-or-
        /// <see langword="null" /> to use the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default" />.</param>
        /// <returns>The zero-based index of <paramref name="item" /> in the sorted <see cref="T:System.Collections.Generic.List`1" />, if <paramref name="item" /> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item" /> or, if there is no larger element, the bitwise complement of <see cref="P:System.Collections.Generic.List`1.Count" />.</returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// <paramref name="comparer" /> is <see langword="null" />, and the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default" /> cannot find an implementation of the <see cref="T:System.IComparable`1" /> generic interface or the <see cref="T:System.IComparable" /> interface for type <paramref name="T" />.</exception>
        public int BinarySearch(Scalar item, IComparer<Scalar> comparer) {
            return Values.BinarySearch(item, comparer);
        }

        /// <summary>Searches the entire sorted <see cref="T:System.Collections.Generic.List`1" /> for an element using the default comparer and returns the zero-based index of the element.</summary>
        /// <param name="item">The object to locate. The value can be <see langword="null" /> for reference types.</param>
        /// <returns>The zero-based index of <paramref name="item" /> in the sorted <see cref="T:System.Collections.Generic.List`1" />, if <paramref name="item" /> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item" /> or, if there is no larger element, the bitwise complement of <see cref="P:System.Collections.Generic.List`1.Count" />.</returns>
        /// <exception cref="T:System.InvalidOperationException">The default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default" /> cannot find an implementation of the <see cref="T:System.IComparable`1" /> generic interface or the <see cref="T:System.IComparable" /> interface for type <paramref name="T" />.</exception>
        public int BinarySearch(Scalar item) {
            return Values.BinarySearch(item);
        }

        /// <summary>Searches a range of elements in the sorted <see cref="T:System.Collections.Generic.List`1" /> for an element using the specified comparer and returns the zero-based index of the element.</summary>
        /// <param name="index">The zero-based starting index of the range to search.</param>
        /// <param name="count">The length of the range to search.</param>
        /// <param name="item">The object to locate. The value can be <see langword="null" /> for reference types.</param>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IComparer`1" /> implementation to use when comparing elements, or <see langword="null" /> to use the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default" />.</param>
        /// <returns>The zero-based index of <paramref name="item" /> in the sorted <see cref="T:System.Collections.Generic.List`1" />, if <paramref name="item" /> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item" /> or, if there is no larger element, the bitwise complement of <see cref="P:System.Collections.Generic.List`1.Count" />.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-
        /// <paramref name="count" /> is less than 0. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="index" /> and <paramref name="count" /> do not denote a valid range in the <see cref="T:System.Collections.Generic.List`1" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// <paramref name="comparer" /> is <see langword="null" />, and the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default" /> cannot find an implementation of the <see cref="T:System.IComparable`1" /> generic interface or the <see cref="T:System.IComparable" /> interface for type <paramref name="T" />.</exception>
        public int BinarySearch(int index, int count, Scalar item, IComparer<Scalar> comparer) {
            return Values.BinarySearch(index, count, item, comparer);
        }

        /// <summary>Removes all elements from the <see cref="T:System.Collections.Generic.List`1" />.</summary>
        public void Clear() {
            Values.Clear();
        }

        /// <summary>Determines the index of a specific item in the <see cref="T:System.Collections.IList" />.</summary>
        /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList" />. </param>
        /// <returns>The index of <paramref name="value" /> if found in the list; otherwise, -1.</returns>
        int IList.IndexOf(object value) {
            return ((IList) Values).IndexOf(value);
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
            ((IList) Values).Insert(index, value);
        }

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList" />.</summary>
        /// <param name="value">The object to remove from the <see cref="T:System.Collections.IList" />. </param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList" /> is read-only.-or- The <see cref="T:System.Collections.IList" /> has a fixed size. </exception>
        void IList.Remove(object value) {
            ((IList) Values).Remove(value);
        }

        /// <summary>Adds an item to the <see cref="T:System.Collections.IList" />.</summary>
        /// <param name="value">The object to add to the <see cref="T:System.Collections.IList" />. </param>
        /// <returns>The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection.</returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList" /> is read-only.-or- The <see cref="T:System.Collections.IList" /> has a fixed size. </exception>
        int IList.Add(object value) {
            return ((IList) Values).Add(value);
        }

        /// <summary>Determines whether the <see cref="T:System.Collections.IList" /> contains a specific value.</summary>
        /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList" />. </param>
        /// <returns>
        /// <see langword="true" /> if the <see cref="T:System.Object" /> is found in the <see cref="T:System.Collections.IList" />; otherwise, <see langword="false" />.</returns>
        public bool Contains(object value) {
            return ((IList) Values).Contains(value);
        }

        /// <summary>Determines whether an element is in the <see cref="T:System.Collections.Generic.List`1" />.</summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1" />. The value can be <see langword="null" /> for reference types.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.List`1" />; otherwise, <see langword="false" />.</returns>
        public bool Contains(Scalar item) {
            return Values.Contains(item);
        }

        /// <summary>Converts the elements in the current <see cref="T:System.Collections.Generic.List`1" /> to another type, and returns a list containing the converted elements.</summary>
        /// <param name="converter">A <see cref="T:System.Converter`2" /> delegate that converts each element from one type to another type.</param>
        /// <typeparam name="TOutput">The type of the elements of the target array.</typeparam>
        /// <returns>A <see cref="T:System.Collections.Generic.List`1" /> of the target type containing the converted elements from the current <see cref="T:System.Collections.Generic.List`1" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="converter" /> is <see langword="null" />.</exception>
        public List<TOutput> ConvertAll<TOutput>(Converter<Scalar, TOutput> converter) {
            return Values.ConvertAll(converter);
        }

        /// <summary>Copies the entire <see cref="T:System.Collections.Generic.List`1" /> to a compatible one-dimensional array, starting at the beginning of the target array.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.List`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.List`1" /> is greater than the number of elements that the destination <paramref name="array" /> can contain.</exception>
        public void CopyTo(Scalar[] array) {
            Values.CopyTo(array);
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
        public void CopyTo(System.Array array, int index) {
            ((ICollection) Values).CopyTo(array, index);
        }

        /// <summary>Copies a range of elements from the <see cref="T:System.Collections.Generic.List`1" /> to a compatible one-dimensional array, starting at the specified index of the target array.</summary>
        /// <param name="index">The zero-based index in the source <see cref="T:System.Collections.Generic.List`1" /> at which copying begins.</param>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.List`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-
        /// <paramref name="arrayIndex" /> is less than 0.-or-
        /// <paramref name="count" /> is less than 0. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="index" /> is equal to or greater than the <see cref="P:System.Collections.Generic.List`1.Count" /> of the source <see cref="T:System.Collections.Generic.List`1" />.-or-The number of elements from <paramref name="index" /> to the end of the source <see cref="T:System.Collections.Generic.List`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />. </exception>
        public void CopyTo(int index, Scalar[] array, int arrayIndex, int count) {
            Values.CopyTo(index, array, arrayIndex, count);
        }

        /// <summary>Copies the entire <see cref="T:System.Collections.Generic.List`1" /> to a compatible one-dimensional array, starting at the specified index of the target array.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.List`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.List`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.</exception>
        public void CopyTo(Scalar[] array, int arrayIndex) {
            Values.CopyTo(array, arrayIndex);
        }

        /// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Generic.List`1" />.</summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.List`1" />.</returns>
        public int Count => Values.Count;

        /// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</summary>
        /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</returns>
        object ICollection.SyncRoot => ((ICollection) Values).SyncRoot;

        /// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).</summary>
        /// <returns>
        /// <see langword="true" /> if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, <see langword="false" />.</returns>
        bool ICollection.IsSynchronized => ((ICollection) Values).IsSynchronized;

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</summary>
        /// <returns>
        /// <see langword="true" /> if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, <see langword="false" />.</returns>
        bool ICollection<Scalar>.IsReadOnly => false;

        /// <summary>Determines whether the <see cref="T:System.Collections.Generic.List`1" /> contains elements that match the conditions defined by the specified predicate.</summary>
        /// <param name="match">The <see cref="T:System.Predicate`1" /> delegate that defines the conditions of the elements to search for.</param>
        /// <returns>
        /// <see langword="true" /> if the <see cref="T:System.Collections.Generic.List`1" /> contains one or more elements that match the conditions defined by the specified predicate; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="match" /> is <see langword="null" />.</exception>
        public bool Exists(Predicate<Scalar> match) {
            return Values.Exists(match);
        }

        /// <summary>Searches for an element that matches the conditions defined by the specified predicate, and returns the first occurrence within the entire <see cref="T:System.Collections.Generic.List`1" />.</summary>
        /// <param name="match">The <see cref="T:System.Predicate`1" /> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The first element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type <paramref name="T" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="match" /> is <see langword="null" />.</exception>
        public Scalar Find(Predicate<Scalar> match) {
            return Values.Find(match);
        }

        /// <summary>Retrieves all the elements that match the conditions defined by the specified predicate.</summary>
        /// <param name="match">The <see cref="T:System.Predicate`1" /> delegate that defines the conditions of the elements to search for.</param>
        /// <returns>A <see cref="T:System.Collections.Generic.List`1" /> containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty <see cref="T:System.Collections.Generic.List`1" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="match" /> is <see langword="null" />.</exception>
        public List<Scalar> FindAll(Predicate<Scalar> match) {
            return Values.FindAll(match);
        }

        /// <summary>Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the entire <see cref="T:System.Collections.Generic.List`1" />.</summary>
        /// <param name="match">The <see cref="T:System.Predicate`1" /> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match" />, if found; otherwise, –1.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="match" /> is <see langword="null" />.</exception>
        public int FindIndex(Predicate<Scalar> match) {
            return Values.FindIndex(match);
        }

        /// <summary>Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1" /> that starts at the specified index and contains the specified number of elements.</summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match">The <see cref="T:System.Predicate`1" /> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match" />, if found; otherwise, –1.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="match" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1" />.-or-
        /// <paramref name="count" /> is less than 0.-or-
        /// <paramref name="startIndex" /> and <paramref name="count" /> do not specify a valid section in the <see cref="T:System.Collections.Generic.List`1" />.</exception>
        public int FindIndex(int startIndex, int count, Predicate<Scalar> match) {
            return Values.FindIndex(startIndex, count, match);
        }

        /// <summary>Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1" /> that extends from the specified index to the last element.</summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="match">The <see cref="T:System.Predicate`1" /> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match" />, if found; otherwise, –1.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="match" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1" />.</exception>
        public int FindIndex(int startIndex, Predicate<Scalar> match) {
            return Values.FindIndex(startIndex, match);
        }

        /// <summary>Searches for an element that matches the conditions defined by the specified predicate, and returns the last occurrence within the entire <see cref="T:System.Collections.Generic.List`1" />.</summary>
        /// <param name="match">The <see cref="T:System.Predicate`1" /> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The last element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type <paramref name="T" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="match" /> is <see langword="null" />.</exception>
        public Scalar FindLast(Predicate<Scalar> match) {
            return Values.FindLast(match);
        }

        /// <summary>Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1" /> that extends from the first element to the specified index.</summary>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="match">The <see cref="T:System.Predicate`1" /> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match" />, if found; otherwise, –1.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="match" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1" />.</exception>
        public int FindLastIndex(int startIndex, Predicate<Scalar> match) {
            return Values.FindLastIndex(startIndex, match);
        }

        /// <summary>Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1" /> that contains the specified number of elements and ends at the specified index.</summary>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match">The <see cref="T:System.Predicate`1" /> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match" />, if found; otherwise, –1.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="match" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1" />.-or-
        /// <paramref name="count" /> is less than 0.-or-
        /// <paramref name="startIndex" /> and <paramref name="count" /> do not specify a valid section in the <see cref="T:System.Collections.Generic.List`1" />.</exception>
        public int FindLastIndex(int startIndex, int count, Predicate<Scalar> match) {
            return Values.FindLastIndex(startIndex, count, match);
        }

        /// <summary>Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the entire <see cref="T:System.Collections.Generic.List`1" />.</summary>
        /// <param name="match">The <see cref="T:System.Predicate`1" /> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match" />, if found; otherwise, –1.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="match" /> is <see langword="null" />.</exception>
        public int FindLastIndex(Predicate<Scalar> match) {
            return Values.FindLastIndex(match);
        }

        /// <summary>Performs the specified action on each element of the <see cref="T:System.Collections.Generic.List`1" />.</summary>
        /// <param name="action">The <see cref="T:System.Action`1" /> delegate to perform on each element of the <see cref="T:System.Collections.Generic.List`1" />.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="action" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">An element in the collection has been modified. This exception is thrown starting with the .NET Framework 4.5. </exception>
        public void ForEach(Action<Scalar> action) {
            Values.ForEach(action);
        }

        /// <summary>Creates a shallow copy of a range of elements in the source <see cref="T:System.Collections.Generic.List`1" />.</summary>
        /// <param name="index">The zero-based <see cref="T:System.Collections.Generic.List`1" /> index at which the range starts.</param>
        /// <param name="count">The number of elements in the range.</param>
        /// <returns>A shallow copy of a range of elements in the source <see cref="T:System.Collections.Generic.List`1" />.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-
        /// <paramref name="count" /> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="index" /> and <paramref name="count" /> do not denote a valid range of elements in the <see cref="T:System.Collections.Generic.List`1" />.</exception>
        public List<Scalar> GetRange(int index, int count) {
            return Values.GetRange(index, count);
        }

        /// <summary>Searches for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="T:System.Collections.Generic.List`1" />.</summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1" />. The value can be <see langword="null" /> for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item" /> within the entire <see cref="T:System.Collections.Generic.List`1" />, if found; otherwise, –1.</returns>
        public int IndexOf(Scalar item) {
            return Values.IndexOf(item);
        }

        /// <summary>Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1" /> that extends from the specified index to the last element.</summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1" />. The value can be <see langword="null" /> for reference types.</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item" /> within the range of elements in the <see cref="T:System.Collections.Generic.List`1" /> that extends from <paramref name="index" /> to the last element, if found; otherwise, –1.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1" />.</exception>
        public int IndexOf(Scalar item, int index) {
            return Values.IndexOf(item, index);
        }

        /// <summary>Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1" /> that starts at the specified index and contains the specified number of elements.</summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1" />. The value can be <see langword="null" /> for reference types.</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item" /> within the range of elements in the <see cref="T:System.Collections.Generic.List`1" /> that starts at <paramref name="index" /> and contains <paramref name="count" /> number of elements, if found; otherwise, –1.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1" />.-or-
        /// <paramref name="count" /> is less than 0.-or-
        /// <paramref name="index" /> and <paramref name="count" /> do not specify a valid section in the <see cref="T:System.Collections.Generic.List`1" />.</exception>
        public int IndexOf(Scalar item, int index, int count) {
            return Values.IndexOf(item, index, count);
        }

        /// <summary>Inserts an element into the <see cref="T:System.Collections.Generic.List`1" /> at the specified index.</summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be <see langword="null" /> for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-
        /// <paramref name="index" /> is greater than <see cref="P:System.Collections.Generic.List`1.Count" />.</exception>
        public void Insert(int index, Scalar item) {
            Values.Insert(index, item);
        }

        /// <summary>Inserts the elements of a collection into the <see cref="T:System.Collections.Generic.List`1" /> at the specified index.</summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the <see cref="T:System.Collections.Generic.List`1" />. The collection itself cannot be <see langword="null" />, but it can contain elements that are <see langword="null" />, if type <paramref name="T" /> is a reference type.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="collection" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-
        /// <paramref name="index" /> is greater than <see cref="P:System.Collections.Generic.List`1.Count" />.</exception>
        public void InsertRange(int index, IEnumerable<Scalar> collection) {
            Values.InsertRange(index, collection);
        }

        /// <summary>Searches for the specified object and returns the zero-based index of the last occurrence within the entire <see cref="T:System.Collections.Generic.List`1" />.</summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1" />. The value can be <see langword="null" /> for reference types.</param>
        /// <returns>The zero-based index of the last occurrence of <paramref name="item" /> within the entire the <see cref="T:System.Collections.Generic.List`1" />, if found; otherwise, –1.</returns>
        public int LastIndexOf(Scalar item) {
            return Values.LastIndexOf(item);
        }

        /// <summary>Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1" /> that extends from the first element to the specified index.</summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1" />. The value can be <see langword="null" /> for reference types.</param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <returns>The zero-based index of the last occurrence of <paramref name="item" /> within the range of elements in the <see cref="T:System.Collections.Generic.List`1" /> that extends from the first element to <paramref name="index" />, if found; otherwise, –1.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1" />. </exception>
        public int LastIndexOf(Scalar item, int index) {
            return Values.LastIndexOf(item, index);
        }

        /// <summary>Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1" /> that contains the specified number of elements and ends at the specified index.</summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1" />. The value can be <see langword="null" /> for reference types.</param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>The zero-based index of the last occurrence of <paramref name="item" /> within the range of elements in the <see cref="T:System.Collections.Generic.List`1" /> that contains <paramref name="count" /> number of elements and ends at <paramref name="index" />, if found; otherwise, –1.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1" />.-or-
        /// <paramref name="count" /> is less than 0.-or-
        /// <paramref name="index" /> and <paramref name="count" /> do not specify a valid section in the <see cref="T:System.Collections.Generic.List`1" />. </exception>
        public int LastIndexOf(Scalar item, int index, int count) {
            return Values.LastIndexOf(item, index, count);
        }

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.List`1" />.</summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.List`1" />. The value can be <see langword="null" /> for reference types.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="item" /> is successfully removed; otherwise, <see langword="false" />.  This method also returns <see langword="false" /> if <paramref name="item" /> was not found in the <see cref="T:System.Collections.Generic.List`1" />.</returns>
        public bool Remove(Scalar item) {
            return Values.Remove(item);
        }

        /// <summary>Removes all the elements that match the conditions defined by the specified predicate.</summary>
        /// <param name="match">The <see cref="T:System.Predicate`1" /> delegate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements removed from the <see cref="T:System.Collections.Generic.List`1" /> .</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="match" /> is <see langword="null" />.</exception>
        public int RemoveAll(Predicate<Scalar> match) {
            return Values.RemoveAll(match);
        }

        /// <summary>Removes the element at the specified index of the <see cref="T:System.Collections.Generic.List`1" />.</summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-
        /// <paramref name="index" /> is equal to or greater than <see cref="P:System.Collections.Generic.List`1.Count" />.</exception>
        public void RemoveAt(int index) {
            Values.RemoveAt(index);
        }

        /// <summary>Gets or sets the element at the specified index.</summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1" /> is read-only.</exception>
        Scalar IList<Scalar>.this[int index] {
            get => Values[index];
            set => Values[index] = value;
        }

        /// <summary>Removes a range of elements from the <see cref="T:System.Collections.Generic.List`1" />.</summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-
        /// <paramref name="count" /> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="index" /> and <paramref name="count" /> do not denote a valid range of elements in the <see cref="T:System.Collections.Generic.List`1" />.</exception>
        public void RemoveRange(int index, int count) {
            Values.RemoveRange(index, count);
        }

        /// <summary>Reverses the order of the elements in the entire <see cref="T:System.Collections.Generic.List`1" />.</summary>
        public void Reverse() {
            Values.Reverse();
        }

        /// <summary>Reverses the order of the elements in the specified range.</summary>
        /// <param name="index">The zero-based starting index of the range to reverse.</param>
        /// <param name="count">The number of elements in the range to reverse.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-
        /// <paramref name="count" /> is less than 0. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="index" /> and <paramref name="count" /> do not denote a valid range of elements in the <see cref="T:System.Collections.Generic.List`1" />. </exception>
        public void Reverse(int index, int count) {
            Values.Reverse(index, count);
        }

        /// <summary>Sorts the elements in the entire <see cref="T:System.Collections.Generic.List`1" /> using the default comparer.</summary>
        /// <exception cref="T:System.InvalidOperationException">The default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default" /> cannot find an implementation of the <see cref="T:System.IComparable`1" /> generic interface or the <see cref="T:System.IComparable" /> interface for type <paramref name="T" />.</exception>
        public void Sort() {
            Values.Sort();
        }

        /// <summary>Sorts the elements in a range of elements in <see cref="T:System.Collections.Generic.List`1" /> using the specified comparer.</summary>
        /// <param name="index">The zero-based starting index of the range to sort.</param>
        /// <param name="count">The length of the range to sort.</param>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IComparer`1" /> implementation to use when comparing elements, or <see langword="null" /> to use the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default" />.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-
        /// <paramref name="count" /> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="index" /> and <paramref name="count" /> do not specify a valid range in the <see cref="T:System.Collections.Generic.List`1" />.-or-The implementation of <paramref name="comparer" /> caused an error during the sort. For example, <paramref name="comparer" /> might not return 0 when comparing an item with itself.</exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// <paramref name="comparer" /> is <see langword="null" />, and the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default" /> cannot find implementation of the <see cref="T:System.IComparable`1" /> generic interface or the <see cref="T:System.IComparable" /> interface for type <paramref name="T" />.</exception>
        public void Sort(int index, int count, IComparer<Scalar> comparer) {
            Values.Sort(index, count, comparer);
        }

        /// <summary>Sorts the elements in the entire <see cref="T:System.Collections.Generic.List`1" /> using the specified <see cref="T:System.Comparison`1" />.</summary>
        /// <param name="comparison">The <see cref="T:System.Comparison`1" /> to use when comparing elements.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="comparison" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentException">The implementation of <paramref name="comparison" /> caused an error during the sort. For example, <paramref name="comparison" /> might not return 0 when comparing an item with itself.</exception>
        public void Sort(Comparison<Scalar> comparison) {
            Values.Sort(comparison);
        }

        /// <summary>Sorts the elements in the entire <see cref="T:System.Collections.Generic.List`1" /> using the specified comparer.</summary>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IComparer`1" /> implementation to use when comparing elements, or <see langword="null" /> to use the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default" />.</param>
        /// <exception cref="T:System.InvalidOperationException">
        /// <paramref name="comparer" /> is <see langword="null" />, and the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default" /> cannot find implementation of the <see cref="T:System.IComparable`1" /> generic interface or the <see cref="T:System.IComparable" /> interface for type <paramref name="T" />.</exception>
        /// <exception cref="T:System.ArgumentException">The implementation of <paramref name="comparer" /> caused an error during the sort. For example, <paramref name="comparer" /> might not return 0 when comparing an item with itself.</exception>
        public void Sort(IComparer<Scalar> comparer) {
            Values.Sort(comparer);
        }

        /// <summary>Copies the elements of the <see cref="T:System.Collections.Generic.List`1" /> to a new array.</summary>
        /// <returns>An array containing copies of the elements of the <see cref="T:System.Collections.Generic.List`1" />.</returns>
        public Scalar[] ToArray() {
            return Values.ToArray();
        }

        /// <summary>Sets the capacity to the actual number of elements in the <see cref="T:System.Collections.Generic.List`1" />, if that number is less than a threshold value.</summary>
        public void TrimExcess() {
            Values.TrimExcess();
        }

        /// <summary>Determines whether every element in the <see cref="T:System.Collections.Generic.List`1" /> matches the conditions defined by the specified predicate.</summary>
        /// <param name="match">The <see cref="T:System.Predicate`1" /> delegate that defines the conditions to check against the elements.</param>
        /// <returns>
        /// <see langword="true" /> if every element in the <see cref="T:System.Collections.Generic.List`1" /> matches the conditions defined by the specified predicate; otherwise, <see langword="false" />. If the list has no elements, the return value is <see langword="true" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="match" /> is <see langword="null" />.</exception>
        public bool TrueForAll(Predicate<Scalar> match) {
            return Values.TrueForAll(match);
        }

        #endregion

    }
}