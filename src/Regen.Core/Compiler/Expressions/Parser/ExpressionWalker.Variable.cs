using System.Collections.Generic;
using System.Text.RegularExpressions;
using Regen.DataTypes;
using Regen.Helpers;

namespace Regen.Compiler.Expressions {
    public class VariableDeclarationExpression : Expression {
        private static readonly RegexResult _equalsMatch = "=".AsResult();

        public Identity Name { get; set; }
        public Expression Right { get; set; }

        public override IEnumerable<RegexResult> Matches() {
            foreach (var match in Name.Matches()) {
                yield return match;
            }

            yield return _equalsMatch;

            foreach (var match in Right.Matches()) {
                yield return match;
            }
        }
    }

    public partial class ExpressionWalker {
        public VariableDeclarationExpression ParseVariable() {
            var var = new VariableDeclarationExpression();
            var.Name = StringIdentity.Parse(this);
            IsCurrentOrThrow(ExpressionToken.Equal);
            NextOrThrow();

            var.Right = Expression.ParseExpression(this);
            return var;
        }
    }
}