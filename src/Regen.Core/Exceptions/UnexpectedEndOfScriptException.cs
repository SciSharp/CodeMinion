using System;
using Regen.Compiler;

namespace Regen.Exceptions {
    [Serializable]
    public class UnexpectedEndOfScriptException : Exception {
        public UnexpectedEndOfScriptException() { }

        public UnexpectedEndOfScriptException(Token token, TokenID? expected) : base($"After this expression: '{token.Match.Value}' expected {expected}") { }
        public UnexpectedEndOfScriptException(string message) : base(message) { }
        public UnexpectedEndOfScriptException(string message, Exception inner) : base(message, inner) { }
    }
}