using System.Collections.Generic;
using System.Text.RegularExpressions;
using Regen.Helpers;

namespace Regen.Compiler.Expressions {
    /// <summary>
    ///     Parses identity[params]
    /// </summary>
    public class IndexerCallExpression : Expression {
        private static readonly Match _matchLeft = "[".WrapAsMatch();
        private static readonly Match _matchRight = "]".WrapAsMatch();
        public IdentityExpression Left;
        public ArgumentsExpression Arguments;

        public IndexerCallExpression(IdentityExpression left, ArgumentsExpression arguments) {
            Left = left;
            Arguments = arguments;
        }

        public static Expression Parse(ExpressionWalker ew) {
            var ret = new IndexerCallExpression(IdentityExpression.Parse(ew), ArgumentsExpression.Parse(ew, ExpressionToken.LeftBracet, ExpressionToken.RightBracet, false));
            if (ew.Current.Token == ExpressionToken.Period) {
                return IdentityExpression.Parse(ew, typeof(IndexerCallExpression), ret);
            }
            return ret;
        }

        public override IEnumerable<Match> Matches() {
            foreach (var match in Left.Matches()) {
                yield return match;
            }

            yield return _matchLeft;
            foreach (var match in Arguments.Matches()) {
                yield return match;
            }
            yield return _matchRight;
        }
    }
}