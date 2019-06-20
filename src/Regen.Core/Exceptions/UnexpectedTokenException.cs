using System;
using Regen.Compiler;
using Regen.Compiler.Digest;
using Regen.Compiler.Expressions;
using Regen.Compiler.Helpers;

namespace Regen.Exceptions {
    [Serializable]
    public class UnexpectedTokenException<T> : RegenException {
        public UnexpectedTokenException(TokenBase<T> dToken, T expected) : base($"After this expression: '{dToken.Match.Value}' expected {expected}") { }
        public UnexpectedTokenException(TokenBase<T> dToken) : base($"After this expression: '{dToken.Match.Value}'") { }
        public UnexpectedTokenException(string message) : base(message) { }
        public UnexpectedTokenException(string message, Exception inner) : base(message, inner) { }
    }
}