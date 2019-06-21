using System.Collections.Generic;
using System.Text.RegularExpressions;
using Regen.Helpers;

namespace Regen.Compiler.Expressions {
    public class NewExpression : Expression {
        private static readonly Match _newMatch = "new".WrapAsMatch();

        public Expression Constructor;

        public NewExpression(CallExpression constructor) {
            Constructor = constructor;
        }

        private NewExpression(Expression exp) {
            Constructor = exp;
        }

        public static NewExpression Parse(ExpressionWalker ew) {
            ew.IsCurrentOrThrow(ExpressionToken.New);
            ew.NextOrThrow();
            return new NewExpression(CallExpression.Parse(ew));
        }

        public override IEnumerable<Match> Matches() {
            yield return _newMatch;
            foreach (var match in Constructor.Matches()) {
                yield return match;
            }
        }
    }
}