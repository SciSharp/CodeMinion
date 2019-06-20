using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Regen.Compiler.Expressions {
    [DebuggerDisplay("{ExpressionToken} - {Match}")]
    public class EToken {
        public ExpressionToken ExpressionToken { get; set; }
        public Match Match { get; set; }

        public EToken() { }

        public EToken(ExpressionToken expressionToken, Match match) {
            ExpressionToken = expressionToken;
            Match = match ?? throw new ArgumentNullException(nameof(match));
        }
    }
}