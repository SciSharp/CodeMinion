using System;
using Regen.Compiler;

namespace Regen.Exceptions {
    [Serializable]
    public class ExpressionCompileException : RegenException {
        public ExpressionCompileException() { }

        public ExpressionCompileException(Token token, TokenID? expected) : base($"After this expression: '{token.Match.Value}' expected {expected}") { }
        public ExpressionCompileException(string message) : base(message) { }
        public ExpressionCompileException(string message, Exception inner) : base(message, inner) { }
    }
}