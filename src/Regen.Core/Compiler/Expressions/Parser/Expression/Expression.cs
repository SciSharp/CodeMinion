namespace Regen.Compiler.Expressions {
    public class Expression {
        public static EmptyExpression None { get; } = new EmptyExpression();
    }

    public class EmptyExpression : Expression { }
}