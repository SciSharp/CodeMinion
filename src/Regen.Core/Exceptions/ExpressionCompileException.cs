using System;
using Regen.Compiler;

namespace Regen.Exceptions {
    [Serializable]
    public class ExpressionCompileException : RegenException {
        public ExpressionCompileException() { }

        public ExpressionCompileException(DToken dToken, DigestToken? expected) : base($"After this expression: '{dToken.Match.Value}' expected {expected}") { }
        public ExpressionCompileException(string message) : base(message) { }
        public ExpressionCompileException(string message, Exception inner) : base(message, inner) { }
    }
}