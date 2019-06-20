namespace Regen.Compiler.Expressions {
    public class PropertyIdentity : Identity {
        public Expression Left { get; set; }
        public Expression Right { get; set; }

        public PropertyIdentity(Expression left, Expression right) {
            Left = left;
            Right = right;
        }
    }
}