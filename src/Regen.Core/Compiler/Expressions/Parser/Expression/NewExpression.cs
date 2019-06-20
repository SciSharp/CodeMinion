namespace Regen.Compiler.Expressions {
    public class NewExpression : Expression {
        public Expression Constructor;

        public NewExpression(CallExpression constructor) {
            Constructor = constructor;
        }

        private NewExpression(Expression exp) {
            Constructor = exp;
        }

        public static NewExpression Parse(ExpressionWalker ew) {
            ew.IsCurrentOrThrow(ExpressionToken.New);
            ew.NextOrThrow();
            return new NewExpression(CallExpression.Parse(ew));
        }
    }
}