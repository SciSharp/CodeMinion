using System;
using Regen.Compiler;

namespace Regen.Exceptions {
    [Serializable]
    public class UndefinedReferenceException : UnexpectedTokenException {
        public UndefinedReferenceException() { }

        public UndefinedReferenceException(DToken dToken, DigestToken? expected) : base($"After this expression: '{dToken.Match.Value}' expected {expected}") { }
        public UndefinedReferenceException(string message) : base(message) { }
        public UndefinedReferenceException(string message, Exception inner) : base(message, inner) { }
    }
}