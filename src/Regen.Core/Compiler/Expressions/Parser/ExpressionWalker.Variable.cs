using System.Collections.Generic;
using System.Text.RegularExpressions;
using Regen.DataTypes;
using Regen.Helpers;

namespace Regen.Compiler.Expressions {
    public class VariableExpression : Expression {
        private static readonly Match _equalsMatch = "=".WrapAsMatch();

        public Identity Name { get; set; }
        public Expression Right { get; set; }

        public override IEnumerable<Match> Matches() {
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