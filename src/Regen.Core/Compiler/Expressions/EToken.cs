using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Regen.Compiler.Helpers;

namespace Regen.Compiler.Expressions {
    [DebuggerDisplay("{Token} - {Match}")]
    public class EToken : TokenBase<ExpressionToken> {
        public EToken() { }
        public int WhitespacesAfterMatch { get; set; }
        public EToken(ExpressionToken token, Match match) : base(token, match) { }
    }
}