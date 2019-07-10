using System;
using System.Collections.Generic;
using System.Linq;
using Regen.Exceptions;
using Regen.Flee.PublicTypes;

namespace Regen.DataTypes.Wrappers {

    /// <summary>
    ///     Serves as a wrapper to <see cref="VariableCollection"/>.
    /// </summary>
    public class VariableCollectionWrapper {
        private readonly VariableCollection _collection;

        public VariableCollectionWrapper(VariableCollection collection) {
            _collection = collection;
        }

        public Array Values() => Array.Create(_collection.Values);

        public Array Keys() => Array.Create(_collection.Keys.Select(k => new StringScalar(k)));

        public Data Get(string key) {
            try {
                return Data.Create(this[key]);
            } catch (ArgumentException e) {
                throw new UndefinedReferenceException(e.Message, e);
            }
        }

        public void Set(string key, object value) {
            this[key] = Data.Create(this[key]);
        }

        #region Delegating

        /// <summary>Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public void Add(KeyValuePair<string, object> item) {
            _collection.Add(item.Key, item.Value);
        }

        /// <summary>Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.</summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
        public void Add(string name, object value) {
            _collection.Add(name, value);
        }

        /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. </exception>
        public void Clear() {
            _collection.Clear();
        }

        /// <summary>Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.</summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />.</returns>
        public bool Contains(KeyValuePair<string, object> item) {
            return _collection.Contains(item);
        }

        /// <summary>Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key.</summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
        /// <returns>
        /// <see langword="true" /> if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the key; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is <see langword="null" />.</exception>
        public bool ContainsKey(string name) {
            return _collection.ContainsKey(name);
        }


        /// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public int Count => _collection.Count;

        public void DefineVariable(string name, Type variableType) {
            _collection.DefineVariable(name, variableType);
        }

        public T GetFunctionResultInternal<T>(string name, object[] arguments) {
            return _collection.GetFunctionResultInternal<T>(name, arguments);
        }

        public Type GetVariableType(string name) {
            return _collection.GetVariableType(name);
        }

        public T GetVariableValueInternal<T>(string name) {
            return _collection.GetVariableValueInternal<T>(name);
        }

        public T GetVirtualPropertyValueInternal<T>(string name, object component) {
            return _collection.GetVirtualPropertyValueInternal<T>(name, component);
        }

        public event EventHandler<InvokeFunctionEventArgs> InvokeFunction {
            add => _collection.InvokeFunction += value;
            remove => _collection.InvokeFunction -= value;
        }

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</summary>
        /// <returns>
        /// <see langword="true" /> if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, <see langword="false" />.</returns>
        public bool IsReadOnly => _collection.IsReadOnly;

        /// <summary>Gets or sets the element with the specified key.</summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>The element with the specified key.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key" /> is not found.</exception>
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
        public object this[string name] {
            get => _collection[name];
            set => _collection[name] = value;
        }

        /// <summary>Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.</summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// <see langword="true" /> if the element is successfully removed; otherwise, <see langword="false" />.  This method also returns <see langword="false" /> if <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
        public bool Remove(string name) {
            return _collection.Remove(name);
        }

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public bool Remove(KeyValuePair<string, object> item) {
            return _collection.Remove(item.Key);
        }

        public event EventHandler<ResolveFunctionEventArgs> ResolveFunction {
            add => _collection.ResolveFunction += value;
            remove => _collection.ResolveFunction -= value;
        }

        public event EventHandler<ResolveVariableTypeEventArgs> ResolveVariableType {
            add => _collection.ResolveVariableType += value;
            remove => _collection.ResolveVariableType -= value;
        }

        public event EventHandler<ResolveVariableValueEventArgs> ResolveVariableValue {
            add => _collection.ResolveVariableValue += value;
            remove => _collection.ResolveVariableValue -= value;
        }

        /// <summary>Gets the value associated with the specified key.</summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// <see langword="true" /> if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is <see langword="null" />.</exception>
        public bool TryGetValue(string key, out object value) {
            return _collection.TryGetValue(key, out value);
        }

        #endregion
    }
}