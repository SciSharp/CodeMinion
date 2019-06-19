using System;
using System.Collections.Generic;

namespace Regen.Helpers {
    public static class DictionaryExtensions {
        public static Dictionary<T, U> AddRange<T, U>(this Dictionary<T, U> destination, Dictionary<T, U> source) {
            if (destination == null) destination = new Dictionary<T, U>();
            foreach (var e in source)
                destination.Add(e.Key, e.Value);
            return destination;
        }
    }
}