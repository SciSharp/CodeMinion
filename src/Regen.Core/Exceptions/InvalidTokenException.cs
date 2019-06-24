using System;

namespace Regen.Exceptions {
    [Serializable]
    public class InvalidTokenException : Exception {
        public InvalidTokenException(Enum expected, Enum got, string details=null) : base($"Expected token of type {expected}, but got {got}: {details}") { }
        public InvalidTokenException(Enum got, string details = null) : base($"Expected a different token, but got {got}: {details}") { }
    }
}