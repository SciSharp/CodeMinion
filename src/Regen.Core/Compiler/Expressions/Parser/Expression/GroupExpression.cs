using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics.Eventing.Reader;
using System.Text.RegularExpressions;
using Regen.Exceptions;
using Regen.Helpers;

namespace Regen.Compiler.Expressions {
    public class GroupExpression : Expression {
        private Match _matchLeft;
        private Match _matchRight;
        public Expression InnerExpression { get; set; }

        public static GroupExpression Parse(ExpressionWalker ew, ExpressionToken left, ExpressionToken right) {
            var grp = new GroupExpression() {_matchLeft = left.GetAttribute<ExpressionTokenAttribute>().Emit.WrapAsMatch(), _matchRight = right.GetAttribute<ExpressionTokenAttribute>().Emit.WrapAsMatch()};
            ew.IsCurrentOrThrow(left);

            ew.NextOrThrow();
            if (ew.Current.Token == right)
                throw new UnexpectedTokenException<ExpressionToken>($"Expected an expression, found end of group of type {right}");

            grp.InnerExpression = ew.ParseExpression();
            ew.IsCurrentOrThrow(right);
            ew.Next();
            return grp;
        }

        public override IEnumerable<Match> Matches() {
            yield return _matchLeft;
            foreach (var match in InnerExpression.Matches()) {
                yield return match;
            }

            yield return _matchRight;
        }
    }
}