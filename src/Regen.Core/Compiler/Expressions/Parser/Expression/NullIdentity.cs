using System.Collections.Generic;
using System.Text.RegularExpressions;
using Regen.Helpers;

namespace Regen.Compiler.Expressions {
    public class NullIdentity : Identity {
        private static readonly Match _nullMatch = "null".WrapAsMatch();
        public static NullIdentity Instance = new NullIdentity();

        public static Expression Parse(ExpressionWalker ew) {
            ew.IsCurrentOrThrow(ExpressionToken.Null);
            ew.Next();
            return new IdentityExpression(Instance);
        }

        public override IEnumerable<Match> Matches() {
            yield return _nullMatch;
        }
    }
}