using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Regen.Compiler.Helpers {
    [DebuggerDisplay("{Token} - {Match}")]
    public abstract class TokenBase<T> {
        public T Token { get; set; }
        public Match Match { get; set; }

        public TokenBase() { }

        public TokenBase(T token, Match match) {
            Token = token;
            Match = match ?? throw new ArgumentNullException(nameof(match));
        }
    }
}