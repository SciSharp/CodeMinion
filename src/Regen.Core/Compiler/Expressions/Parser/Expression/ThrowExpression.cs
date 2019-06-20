namespace Regen.Compiler.Expressions {
    public class ThrowExpression : Expression {
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
            ret.Right = ew.ParseExpression(typeof(ThrowExpression));
            return ret;
        }
    }
}