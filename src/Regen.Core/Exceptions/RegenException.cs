using System;

namespace Regen.Exceptions {
    /// <summary>
    ///     Serves as a base exception to all <see cref="Regen.Exceptions"/> namespace.
    /// </summary>
    [Serializable]
    public class RegenException : Exception {
        public RegenException() { }

        public RegenException(string message) : base(message) { }
        public RegenException(string message, Exception inner) : base(message, inner) { }
    }
}