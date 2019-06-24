using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Regen.Helpers;

namespace Regen.Parser.Expressions {
    public class RightOperatorExpression : Expression, IOperatorExpression {
        public Expression Left;
        public ExpressionToken Op;

        public RightOperatorExpression(Expression left, ExpressionToken op) {
            Left = left;
            Op = op;
        }

        public static bool IsNextAnRightUniOperation(ExpressionWalker ew, Type caller) {
            if (!ew.HasNext)
                return false;

            using (ew.CheckPoint()) {
                ew.Next();
                return IsCurrentAnRightUniOperation(ew);
            }
        }

        public static bool IsCurrentAnRightUniOperation(ExpressionWalker ew) {
            switch (ew.Current.Token) {
                case ExpressionToken.Increment:
                case ExpressionToken.Decrement:
                case ExpressionToken.NullCoalescing:
                    return true;
                default:
                    return false;
            }
        }

        public static Expression Parse(ExpressionWalker ew, Expression known = null) {
            var left = known ?? ParseExpression(ew, typeof(RightOperatorExpression));

            if (!IsCurrentAnRightUniOperation(ew))
                return left;

            var op = ew.Current.Token;
            ew.NextOrThrow();
            return new RightOperatorExpression(left, op);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ExpressionToken IOperatorExpression.Op {
            get => Op;
            set => Op = value;
        }

        public override IEnumerable<RegexResult> Matches() {
            foreach (var match in Left.Matches()) {
                yield return match;
            }

            yield return AttributeExtensions.GetAttribute<ExpressionTokenAttribute>(Op).Emit.AsResult();
        }

        public override IEnumerable<Expression> Iterate() {
            return this.Yield().Concat(Left.Iterate());
        }
    }
}