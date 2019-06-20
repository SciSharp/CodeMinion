using System.Diagnostics;
using System.Text.RegularExpressions;
using Regen.Compiler.Digest;

namespace Regen.Compiler.Helpers {
    [DebuggerDisplay("{Token} - {Match}")]
    public class DToken : TokenBase<DigestToken> {
        public DToken() { }
        public DToken(DigestToken token, Match match) : base(token, match) { }
    }
}