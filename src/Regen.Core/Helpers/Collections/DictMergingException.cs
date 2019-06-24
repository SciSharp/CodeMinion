using System;

namespace Regen.Helpers.Collections {
    [Serializable]
    public class DictMergingException : Exception {
        public DictMergingException() { }
        public DictMergingException(string message) : base(message) { }
        public DictMergingException(string message, Exception inner) : base(message, inner) { }
    }
}