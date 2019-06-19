using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Regen.Compiler {
    [DebuggerDisplay("{TokenId} - {Match}")]
    public class Token {
        public TokenID TokenId { get; set; }
        public Match Match { get; set; }

        public Token() { }

        public Token(TokenID tokenId, Match match) {
            TokenId = tokenId;
            Match = match ?? throw new ArgumentNullException(nameof(match));
        }
    }
}