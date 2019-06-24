using System.Collections.Generic;
using System.Linq;
using Regen.Helpers;

namespace Regen.Parser.Expressions {
    public class ThrowExpression : Expression {
        private static readonly RegexResult _throwMatch = "throw".AsResult();
        public Expression Right;

        public ThrowExpression(Expression right) {
            Right = right;
        }

        public ThrowExpression() { }

        public static ThrowExpression Parse(ExpressionWalker ew) {
            ew.IsCurrentOrThrow(ExpressionToken.Throw); //you get it? throw if current is not throw.
            ew.NextOrThrow();
            // ReSharper disable once UseObjectOrCollectionInitializer
            var ret = new ThrowExpression();
            ret.Right = Expression.ParseExpression(ew, typeof(ThrowExpression));
            return ret;
        }

        public override IEnumerable<RegexResult> Matches() {
            yield return _throwMatch;

            foreach (var match in Right.Matches()) {
                yield return match;
            }
        }

        public override IEnumerable<Expression> Iterate() {
            return this.Yield().Concat(Right.Iterate()).Concat(Right.Iterate());
        }
    }
}