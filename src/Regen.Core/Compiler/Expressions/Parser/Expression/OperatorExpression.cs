namespace Regen.Compiler.Expressions {
    public class OperatorExpression : Expression {
        public Expression Left;
        public ExpressionToken Op;
        public Expression Right;

        public OperatorExpression(Expression lexpr, ExpressionToken op, Expression rexpr) {
            Left = lexpr;
            Op = op;
            Right = rexpr;
        }

        private OperatorExpression() { }

        public static bool IsNextAnOperation(ExpressionWalker ew) {
            if (!ew.HasNext)
                return false;

            using (ew.CheckPoint()) {
                ew.Next();
                return IsCurrentAnOperation(ew);
            }
        }

        public static bool IsCurrentAnOperation(ExpressionWalker ew) {
            switch (ew.Current.Token) {
                case ExpressionToken.Increment:
                case ExpressionToken.Decrement:
                case ExpressionToken.Add:
                case ExpressionToken.Sub:
                case ExpressionToken.Pow:
                case ExpressionToken.Mul:
                case ExpressionToken.Div:
                case ExpressionToken.DoubleEqual:
                case ExpressionToken.NotEqual:
                case ExpressionToken.Equal:
                case ExpressionToken.DoubleAnd:
                case ExpressionToken.And:
                case ExpressionToken.DoubleOr:
                case ExpressionToken.Or:
                case ExpressionToken.Not:
                case ExpressionToken.Xor:
                case ExpressionToken.ShiftRight:
                case ExpressionToken.BiggerOrEqualThat:
                case ExpressionToken.BiggerThan:
                case ExpressionToken.ShiftLeft:
                case ExpressionToken.SmallerOrEqualThat:
                case ExpressionToken.SmallerThan:
                case ExpressionToken.RangeTo:
                    return true;
                case ExpressionToken.NullCoalescing:
                    return true; //todo make a specific custom op expressonfor null coalescing.
                default:
                    return false;
            }
        }

        public static OperatorExpression Parse(ExpressionWalker ew) {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var ret = new OperatorExpression();
            ret.Left = ew.ParseExpression(typeof(OperatorExpression));
            if (!IsCurrentAnOperation(ew))
                throw new InvalidTokenException(ExpressionToken.Add, ew.Current.Token);
            ret.Op = ew.Current.Token;
            ew.NextOrThrow();
            ret.Right = ew.ParseExpression();
            return ret;
        }
    }
}