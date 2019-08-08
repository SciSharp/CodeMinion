using System.Collections.Generic;
using System.Linq;
using Regen.Helpers;

namespace Regen.Parser.Expressions {
    /// <summary>
    ///     Parses: identity(params)
    /// </summary>
    public class CallExpression : Expression {
        private static readonly RegexResult _matchLeft = "(".AsResult();
        private static readonly RegexResult _matchRight = ")".AsResult();
        public Expression FunctionName;
        public ArgumentsExpression Arguments;

        public CallExpression(Expression functionName, ArgumentsExpression args) {
            FunctionName = functionName;
            Arguments = args;
        }

        public CallExpression(string functionName, params Expression[] args) {
            FunctionName = IdentityExpression.WrapVariable(functionName);
            Arguments = new ArgumentsExpression(args);
        }

        private CallExpression() { }

        public static Expression Parse(ExpressionWalker ew, Expression left = null) {
            var fc = new CallExpression();
            fc.FunctionName = left ?? IdentityExpression.Parse(ew);
            fc.Arguments = ArgumentsExpression.Parse(ew, ExpressionToken.LeftParen, ExpressionToken.RightParen, true);

            return InteractableExpression.TryExpand(fc, ew);
        }

        public override IEnumerable<RegexResult> Matches() {
            foreach (var match in FunctionName.Matches()) {
                yield return match;
            }

            yield return _matchLeft;
            foreach (var match in Arguments.Matches()) {
                yield return match;
            }

            yield return _matchRight;
        }

        public override IEnumerable<Expression> Iterate() {
            return this.Yield().Concat(FunctionName.Iterate()).Concat(Arguments.Iterate());
        }
    }
}