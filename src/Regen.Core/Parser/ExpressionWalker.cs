using System.Collections.Generic;
using System.Diagnostics;
using Regen.Helpers.Collections;

namespace Regen.Parser {
    [DebuggerDisplay("{base.Cursor}->{Current.Token}->{Current.Match}")]
    public partial class ExpressionWalker : TokenWalker<TokenMatch> {
        public ExpressionWalker(IList<TokenMatch> list) : base(list) { }
        protected ExpressionWalker() { }
    }
}