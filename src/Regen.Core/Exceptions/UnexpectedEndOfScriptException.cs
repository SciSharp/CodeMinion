using System;
using Regen.Compiler;
using Regen.Compiler.Expressions;
using Regen.Compiler.Helpers;
using Regen.Parser;

namespace Regen.Exceptions {
    [Serializable]
    public class UnexpectedEndOfScriptException : Exception {
        public UnexpectedEndOfScriptException() { }

        public UnexpectedEndOfScriptException(TokenMatch dTokenMatch, ExpressionToken? expected) : base($"After this expression: '{dTokenMatch.Match.Value}' expected {expected}") { }
        public UnexpectedEndOfScriptException(string message) : base(message) { }
        public UnexpectedEndOfScriptException(string message, Exception inner) : base(message, inner) { }
    }
}