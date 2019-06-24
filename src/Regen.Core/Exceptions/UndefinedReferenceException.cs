using System;
using Regen.Compiler;
using Regen.Compiler.Digest;
using Regen.Compiler.Expressions;
using Regen.Compiler.Helpers;

namespace Regen.Exceptions {
    [Serializable]
    public class UndefinedReferenceException : RegenException {
        public UndefinedReferenceException(TokenMatch dToken, ExpressionToken? expected) : base($"After this expression: '{dToken.Match.Value}' expected {expected}") { }
        public UndefinedReferenceException(string message) : base(message) { }
        public UndefinedReferenceException(string message, Exception inner) : base(message, inner) { }
    }
}