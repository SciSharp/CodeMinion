using System;
using Regen.Compiler;

namespace Regen.Exceptions {
    [Serializable]
    public class UnexpectedTokenException : RegenException {
        public UnexpectedTokenException() { }

        public UnexpectedTokenException(Token token, TokenID? expected) : base($"After this expression: '{token.Match.Value}' expected {expected}") { }
        public UnexpectedTokenException(string message) : base(message) { }
        public UnexpectedTokenException(string message, Exception inner) : base(message, inner) { }
    }
}