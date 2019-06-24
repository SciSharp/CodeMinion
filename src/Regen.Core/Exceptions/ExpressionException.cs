using System;
using Regen.Exceptions;

namespace Regen.Compiler.Expressions {
    public class ExpressionException : RegenException {
        public ExpressionException() { }
        public ExpressionException(string message) : base(message) { }
        public ExpressionException(string message, Exception inner) : base(message, inner) { }
    }
}