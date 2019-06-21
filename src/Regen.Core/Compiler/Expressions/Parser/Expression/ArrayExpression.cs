using System.Collections.Generic;
using System.Text.RegularExpressions;
using Regen.Helpers;

namespace Regen.Compiler.Expressions {
    public class ArrayExpression : Expression {
        private static readonly Match _matchLeft = "[".WrapAsMatch();
        private static readonly Match _matchRight = "]".WrapAsMatch();
        private static readonly Match _seperator = ",".WrapAsMatch();
        public Expression[] Values;

        public ArrayExpression(Expression[] values) {
            Values = values;
        }

        private ArrayExpression() { }

        public static ArrayExpression Parse(ExpressionWalker ew) {
            return new ArrayExpression(ArgumentsExpression.Parse(ew, ExpressionToken.LeftBracet, ExpressionToken.RightBracet, true).Arguments);
        }


        public override IEnumerable<Match> Matches() {
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
    }
}