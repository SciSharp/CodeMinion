using System;
using Regen.Compiler;
using Regen.Compiler.Expressions;
using Regen.Compiler.Helpers;
using Regen.Parser;

namespace Regen.Exceptions {
    [Serializable]
    public class UnexpectedTokenException : RegenException {
        public UnexpectedTokenException(TokenMatch dToken, ExpressionToken? expected) : base($"After this expression: '{dToken.Match.Value}' expected {expected}") { }
        public UnexpectedTokenException(TokenMatch dToken) : base($"After this expression: '{dToken.Match.Value}'") { }
        public UnexpectedTokenException(string message) : base(message) { }
        public UnexpectedTokenException(string message, Exception inner) : base(message, inner) { }
        public UnexpectedTokenException(Enum expected, Enum got, string details = null) : base($"Expected token of type {expected}, but got {got}: {details}") { }
        public UnexpectedTokenException(Enum got, string details = null) : base($"Expected a different token, but got {got}: {details}") { }
    }
}