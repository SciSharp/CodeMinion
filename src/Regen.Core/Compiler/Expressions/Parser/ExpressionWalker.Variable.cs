using Regen.DataTypes;

namespace Regen.Compiler.Expressions {
    public class VariableExpression : Expression {
        public Identity Name { get; set; }
        public Expression Right { get; set; }
    }

    public partial class ExpressionWalker {
        public VariableExpression ParseVariable() {
            var var = new VariableExpression();
            var.Name = StringIdentity.Parse(this);
            IsCurrentOrThrow(ExpressionToken.Equal);
            NextOrThrow();

            var.Right = ParseExpression();
            return var;
        }
    }
}