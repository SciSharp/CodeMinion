using System.Collections.Generic;
using System.Diagnostics;
using Regen.Collections;

namespace Regen.Compiler.Expressions {
    [DebuggerDisplay("{base.Cursor}->{Current.Token}->{Current.Match}")]
    public partial class ExpressionWalker : TokenWalker<EToken> {
        public ExpressionWalker(IList<EToken> list) : base(list) { }
        protected ExpressionWalker() { }
    }
}