using System;
using Regen.Compiler;
using Regen.Compiler.Digest;
using Regen.Compiler.Helpers;

namespace Regen.Exceptions {
    [Serializable]
    public class UnexpectedEndOfScriptException : Exception {
        public UnexpectedEndOfScriptException() { }

        public UnexpectedEndOfScriptException(DToken dToken, DigestToken? expected) : base($"After this expression: '{dToken.Match.Value}' expected {expected}") { }
        public UnexpectedEndOfScriptException(string message) : base(message) { }
        public UnexpectedEndOfScriptException(string message, Exception inner) : base(message, inner) { }
    }
}