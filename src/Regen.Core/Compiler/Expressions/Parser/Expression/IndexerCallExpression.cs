namespace Regen.Compiler.Expressions {
    public class IndexerCallExpression : Expression {
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
    }
}