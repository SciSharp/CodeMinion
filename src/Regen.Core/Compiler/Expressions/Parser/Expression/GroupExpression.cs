using System.Data.SqlTypes;
using System.Diagnostics.Eventing.Reader;
using Regen.Exceptions;

namespace Regen.Compiler.Expressions {
    public class GroupExpression : Expression {
        public Expression InnerExpression { get; set; }

        public static GroupExpression Parse(ExpressionWalker ew, ExpressionToken left, ExpressionToken right) {
            var grp = new GroupExpression();
            ew.IsCurrentOrThrow(left);

            ew.NextOrThrow();
            if (ew.Current.Token == right)
                throw new UnexpectedTokenException<ExpressionToken>($"Expected an expression, found end of group of type {right}");

            grp.InnerExpression = ew.ParseExpression();
            ew.IsCurrentOrThrow(right);
            ew.Next();
            return grp;
        }
    }
}