using System.Collections.Generic;
using System.Linq;
using Regen.Helpers;

namespace Regen.Parser.Expressions {
    /// <summary>
    ///     Parses identity[params]
    /// </summary>
    public class IndexerCallExpression : Expression {
        private static readonly RegexResult _matchLeft = "[".AsResult();
        private static readonly RegexResult _matchRight = "]".AsResult();
        public Expression Left;
        public ArgumentsExpression Arguments;

        public IndexerCallExpression(Expression left, ArgumentsExpression arguments) {
            Left = left;
            Arguments = arguments;
        }

        public static Expression Parse(ExpressionWalker ew, Expression left = null) {
            var ret = new IndexerCallExpression(left ?? IdentityExpression.Parse(ew), ArgumentsExpression.Parse(ew, ExpressionToken.LeftBracet, ExpressionToken.RightBracet, false));
            return InteractableExpression.TryExpand(ret, ew);
        }

        public override IEnumerable<RegexResult> Matches() {
            foreach (var match in Left.Matches()) {
                yield return match;
            }

            yield return _matchLeft;
            foreach (var match in Arguments.Matches()) {
                yield return match;
            }

            yield return _matchRight;
        }

        public override IEnumerable<Expression> Iterate() {
            return this.Yield().Concat(Arguments.Iterate());
        }
    }
}