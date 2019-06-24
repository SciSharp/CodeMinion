using System.Collections.Generic;
using Regen.Helpers;

namespace Regen.Parser.Expressions {
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

        public static VariableDeclarationExpression Parse(ExpressionWalker ew) {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var var = new VariableDeclarationExpression();
            var.Name = StringIdentity.Parse(ew);
            ew.IsCurrentOrThrow(ExpressionToken.Equal);
            ew.NextOrThrow();

            var.Right = Expression.ParseExpression(ew);
            return var;
        }
    }
}