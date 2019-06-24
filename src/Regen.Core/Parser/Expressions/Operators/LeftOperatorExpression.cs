using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Regen.Exceptions;
using Regen.Helpers;

namespace Regen.Parser.Expressions {
    public class LeftOperatorExpression : Expression, IOperatorExpression {
        public ExpressionToken Op;
        public Expression Right;

        public LeftOperatorExpression(ExpressionToken op, Expression right) {
            Op = op;
            Right = right;
        }

        public static bool IsCurrentAnLeftUniOperation(ExpressionWalker ew) {
            switch (ew.Current.Token) {
                case ExpressionToken.Increment:
                case ExpressionToken.Decrement:
                case ExpressionToken.Add:
                case ExpressionToken.Sub:
                case ExpressionToken.Not:
                case ExpressionToken.Xor:
                case ExpressionToken.NotBoolean:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsNextAnLeftUniOperation(ExpressionWalker ew) {
            if (!ew.HasNext)
                return false;

            using (ew.CheckPoint()) {
                ew.Next();
                return IsCurrentAnLeftUniOperation(ew);
            }
        }

        public static LeftOperatorExpression Parse(ExpressionWalker ew, Expression known = null) {
            if (!IsCurrentAnLeftUniOperation(ew))
                throw new UnexpectedTokenException(ExpressionToken.Increment, ew.Current.Token);

            var op = ew.Current.Token;
            ew.NextOrThrow();
            var right = ParseExpression(ew, typeof(LeftOperatorExpression));
            return new LeftOperatorExpression(op, right);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ExpressionToken IOperatorExpression.Op {
            get => Op;
            set => Op = value;
        }


        public override IEnumerable<RegexResult> Matches() {
            yield return AttributeExtensions.GetAttribute<ExpressionTokenAttribute>(Op).Emit.AsResult();
            foreach (var match in Right.Matches()) {
                yield return match;
            }
        }

        public override IEnumerable<Expression> Iterate() {
            return this.Yield().Concat(Right.Iterate());
        }
    }
}