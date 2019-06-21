namespace Regen.Compiler.Expressions {
    public class KeyValueExpression : Expression {
        public Expression Key;
        public Expression Value;

        public KeyValueExpression(Expression key, Expression value) {
            Key = key;
            Value = value;
        }

        public static KeyValueExpression Parse(ExpressionWalker ew, Expression left = null) {
            var key = left ?? ew.ParseExpression(typeof(KeyValueExpression));
            ew.IsCurrentOrThrow(ExpressionToken.Colon);
            ew.NextOrThrow();
            var value = ew.ParseExpression(typeof(KeyValueExpression));
            return new KeyValueExpression(key, value);
        }
    }
}