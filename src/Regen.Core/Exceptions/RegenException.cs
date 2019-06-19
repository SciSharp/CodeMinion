using System;

namespace Regen.Exceptions {
    [Serializable]
    public class RegenException : Exception {
        public RegenException() { }

        public RegenException(string message) : base(message) { }
        public RegenException(string message, Exception inner) : base(message, inner) { }
    }
}