namespace Regen.Compiler.Expressions {
    public class ArrayExpression : Expression {
        public Expression[] Values;

        public ArrayExpression(Expression[] values) {
            Values = values;
        }

        private ArrayExpression() { }

        public static ArrayExpression Parse(ExpressionWalker ew) {
            return new ArrayExpression(ArgumentsExpression.Parse(ew, ExpressionToken.LeftBracet, ExpressionToken.RightBracet, true).Arguments);
        }
    }
}