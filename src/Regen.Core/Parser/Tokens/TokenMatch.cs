using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Regen.Compiler.Helpers;

namespace Regen.Compiler.Expressions {
    [DebuggerDisplay("{Token} - {Match}")]
    public class TokenMatch {
        public int WhitespacesAfterMatch { get; set; }
        public ExpressionToken Token { get; set; }
        public Match Match { get; set; }

        public TokenMatch() { }

        public TokenMatch(ExpressionToken token, Match match) {
            Token = token;
            Match = match ?? throw new ArgumentNullException(nameof(match));
        }
    }
}