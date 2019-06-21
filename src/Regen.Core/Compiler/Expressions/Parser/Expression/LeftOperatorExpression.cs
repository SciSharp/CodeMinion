using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Regen.Helpers;

namespace Regen.Compiler.Expressions {
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
                throw new InvalidTokenException(ExpressionToken.Increment, ew.Current.Token);

            var op = ew.Current.Token;
            ew.NextOrThrow();
            var right = ew.ParseExpression(typeof(LeftOperatorExpression));
            return new LeftOperatorExpression(op, right);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ExpressionToken IOperatorExpression.Op {
            get => Op;
            set => Op = value;
        }


        public override IEnumerable<Match> Matches() {
            yield return Op.GetAttribute<ExpressionTokenAttribute>().Emit.WrapAsMatch();
            foreach (var match in Right.Matches()) {
                yield return match;
            }
        }
    }
}