using System;

namespace Regen.Exceptions {
    public class ExpressionException : RegenException {
        public ExpressionException() { }
        public ExpressionException(string message) : base(message) { }
        public ExpressionException(string message, Exception inner) : base(message, inner) { }
    }
}