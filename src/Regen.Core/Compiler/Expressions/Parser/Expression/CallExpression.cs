using System.Collections.Generic;
using System.Text.RegularExpressions;
using Regen.Helpers;

namespace Regen.Compiler.Expressions {
    /// <summary>
    ///     Parses: identity(params)
    /// </summary>
    public class CallExpression : Expression {
        private static readonly Match _matchLeft = "(".WrapAsMatch();
        private static readonly Match _matchRight = ")".WrapAsMatch();
        public IdentityExpression FunctionName;
        public ArgumentsExpression Arguments;

        public CallExpression(IdentityExpression functionName, ArgumentsExpression args) {
            FunctionName = functionName;
            Arguments = args;
        }

        private CallExpression() { }

        public static Expression Parse(ExpressionWalker ew) {
            var fc = new CallExpression();
            fc.FunctionName = IdentityExpression.Parse(ew);
            fc.Arguments = ArgumentsExpression.Parse(ew, ExpressionToken.LeftParen, ExpressionToken.RightParen, true);
            if (ew.Current.Token == ExpressionToken.Period) {
                return IdentityExpression.Parse(ew, typeof(CallExpression), fc);
            }

            return fc;
        }

        public override IEnumerable<Match> Matches() {
            foreach (var match in FunctionName.Matches()) {
                yield return match;
            }

            yield return _matchLeft;
            foreach (var match in Arguments.Matches()) {
                yield return match;
            }

            yield return _matchRight;
        }
    }
}