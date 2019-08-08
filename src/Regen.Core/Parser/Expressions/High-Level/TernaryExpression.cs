using System.Collections.Generic;
using System.Linq;
using Regen.Helpers;

namespace Regen.Parser.Expressions {
    public class TernaryExpression : Expression {
        private static readonly RegexResult _matchSeperator = "|".AsResult();
        public Expression Condition;
        public Expression IfTrue;
        public Expression IfFalse;

        public TernaryExpression(Expression condition, Expression ifTrue, Expression ifFalse) {
            Condition = condition;
            IfTrue = ifTrue;
            IfFalse = ifFalse;
        }

        public TernaryExpression(Expression condition, Expression ifTrue) {
            Condition = condition;
            IfTrue = ifTrue;
        }

        public static TernaryExpression Parse(ExpressionWalker ew, Expression condition = null) {
            if (condition == null) {
                condition = Expression.ParseExpression(ew, typeof(ExpressionParser));
            }

            ew.IsCurrentOrThrow(ExpressionToken.Or);
            ew.NextOrThrow();
            Expression trueResult;
            Expression falseResult = null;
            if (ew.OptionalCurrent(ExpressionToken.Or)) {
                trueResult = Expression.None;
                falseResult = Expression.ParseExpression(ew, typeof(TernaryExpression));
            } else {
                trueResult = Expression.ParseExpression(ew, typeof(TernaryExpression));
                if (ew.OptionalCurrent(ExpressionToken.Or)) {
                    falseResult = Expression.ParseExpression(ew, typeof(TernaryExpression));
                }
            }

            return new TernaryExpression(condition, trueResult, falseResult);
        }

        /// <summary>
        ///     All matches that were used to assemble this expression, in order.
        /// </summary>
        public override IEnumerable<RegexResult> Matches() {
            foreach (var c in Condition.Matches()) {
                yield return c;
            }

            yield return _matchSeperator;
            foreach (var c in IfTrue.Matches())
                yield return c;

            if (IfFalse != null) {
                yield return _matchSeperator;

                foreach (var c in IfFalse.Matches()) {
                    yield return c;
                }
            }
        }

        /// <summary>
        ///     Iterates all expressions in their evaluation order. (left to right).
        /// </summary>
        public override IEnumerable<Expression> Iterate() {
            var ret = this.Yield().Concat(Condition.Iterate()).Concat(IfTrue.Iterate());
            return IfFalse != null ? ret.Concat(IfFalse.Iterate()) : ret;
        }
    }
}