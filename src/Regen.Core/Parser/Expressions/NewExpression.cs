using System.Collections.Generic;
using System.Linq;
using Regen.Helpers;

namespace Regen.Parser.Expressions {
    public class NewExpression : Expression {
        private static readonly RegexResult _newMatch = "new".AsResult();

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

        public override IEnumerable<RegexResult> Matches() {
            yield return _newMatch;
            foreach (var match in Constructor.Matches()) {
                yield return match;
            }
        }

        public override IEnumerable<Expression> Iterate() {
            return this.Yield().Concat(Constructor.Iterate());
        }
    }
}