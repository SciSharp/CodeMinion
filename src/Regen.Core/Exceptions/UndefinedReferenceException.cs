using System;
using Regen.Compiler;

namespace Regen.Exceptions {
    [Serializable]
    public class UndefinedReferenceException : UnexpectedTokenException {
        public UndefinedReferenceException() { }

        public UndefinedReferenceException(Token token, TokenID? expected) : base($"After this expression: '{token.Match.Value}' expected {expected}") { }
        public UndefinedReferenceException(string message) : base(message) { }
        public UndefinedReferenceException(string message, Exception inner) : base(message, inner) { }
    }
}