using System;

namespace Regen.Compiler.Expressions {
    [Serializable]
    public class InvalidTokenException : Exception {
        public InvalidTokenException(Enum expected, Enum got) : base($"Expected token of type {expected} but got {got}.") { }
    }
}