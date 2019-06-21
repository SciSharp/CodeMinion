using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Regen.Helpers;

namespace Regen.Compiler.Expressions {
    public class ArgumentsExpression : Expression {
        private static readonly Match _seperator = ",".WrapAsMatch();
        public Expression[] Arguments;

        public ArgumentsExpression(params Expression[] arguments) {
            Arguments = arguments;
        }

        private ArgumentsExpression() { }

        public static ArgumentsExpression Parse(ExpressionWalker ew, ExpressionToken left, ExpressionToken right, bool argsOptional) {
            var args = new ArgumentsExpression();
            ew.IsCurrentOrThrow(left);
            ew.NextOrThrow();
            var exprs = new List<Expression>();

            while (ew.Current.Token != right) {
                if (ew.Current.Token == ExpressionToken.Comma) {
                    if (ew.HasBack && ew.PeakBack.Token == ExpressionToken.Comma) {
                        exprs.Add(NullIdentity.Instance);
                    }

                    ew.NextOrThrow();
                    continue;
                }

                var expression = ew.ParseExpression();
                if (ew.IsCurrent(ExpressionToken.Colon)) {
                    //handle keyvalue item
                    exprs.Add(KeyValueExpression.Parse(ew, expression));
                } else
                    exprs.Add(expression);
            }

            if (exprs.Count == 0 && !argsOptional)
                throw new Exception($"Was expecting an expression between {left} and {right}");

            ew.Next();
            args.Arguments = exprs.ToArray();
            return args;
        }

        public override IEnumerable<Match> Matches() {
            for (var i = 0; i < Arguments.Length; i++) {
                foreach (var match in Arguments[i].Matches()) {
                    yield return match;
                }

                if (i < Arguments.Length - 1) { //is not last
                    yield return _seperator;
                }
            }
        }
    }
}