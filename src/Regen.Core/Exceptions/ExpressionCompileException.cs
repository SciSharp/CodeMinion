using System;
using Regen.Compiler;
using Regen.Compiler.Expressions;
using Regen.Compiler.Helpers;
using Regen.Parser;

namespace Regen.Exceptions {
    [Serializable]
    public class ExpressionCompileException : RegenException {
        public ExpressionCompileException() { }

        public ExpressionCompileException(TokenMatch dTokenMatch, ExpressionToken? expected) : base($"After this expression: '{dTokenMatch.Match.Value}' expected {expected}") { }
        public ExpressionCompileException(string message) : base(message) { }
        public ExpressionCompileException(string message, Exception inner) : base(message, inner) { }
    }
}