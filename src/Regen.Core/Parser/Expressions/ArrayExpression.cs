using System.Collections.Generic;
using System.Linq;
using Regen.Helpers;

namespace Regen.Parser.Expressions {
    public class ArrayExpression : Expression {
        private static readonly RegexResult _matchLeft = "[".AsResult();
        private static readonly RegexResult _matchRight = "]".AsResult();
        private static readonly RegexResult _seperator = ",".AsResult();
        public Expression[] Values;

        public ArrayExpression(Expression[] values) {
            Values = values;
        }

        private ArrayExpression() { }

        public static ArrayExpression Parse(ExpressionWalker ew) {
            return new ArrayExpression(ArgumentsExpression.Parse(ew, ExpressionToken.LeftBracet, ExpressionToken.RightBracet, true).Arguments);
        }


        public override IEnumerable<RegexResult> Matches() {
            yield return _matchLeft;
            for (var i = 0; i < Values.Length; i++) {
                foreach (var match in Values[i].Matches()) {
                    yield return match;
                }

                if (i < Values.Length - 1) {
                    yield return _seperator;
                }
            }

            yield return _matchRight;
        }

        public override IEnumerable<Expression> Iterate() {
            return this.Yield().Concat(Values.SelectMany(e => e.Iterate()));
        }
    }
}