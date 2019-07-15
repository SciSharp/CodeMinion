using System;
using System.Collections.Generic;
using System.Linq;
using Regen.Exceptions;
using Regen.Helpers;

namespace Regen.Parser.Expressions {
    public class ArgumentsExpression : Expression {
        private static readonly RegexResult _seperator = ",".AsResult();
        public Expression[] Arguments;

        public ArgumentsExpression(params Expression[] arguments) {
            Arguments = arguments;
        }

        private ArgumentsExpression() { }

        public static ArgumentsExpression Parse(ExpressionWalker ew, ExpressionToken left, ExpressionToken right, bool argsOptional, Type caller = null) {
            var args = new ArgumentsExpression();
            ew.IsCurrentOrThrow(left);
            ew.NextOrThrow();
            var exprs = new List<Expression>();

            while (ew.Current.Token != right && ew.HasNext) {
                if (ew.Current.Token == ExpressionToken.Comma) {
                    if (ew.HasBack && ew.PeakBack.Token == ExpressionToken.Comma) {
                        exprs.Add(NullIdentity.Instance);
                    }

                    ew.NextOrThrow();
                    continue;
                }

                var expression = ParseExpression(ew, caller);
                if (ew.IsCurrent(ExpressionToken.Colon)) {
                    //handle keyvalue item
                    exprs.Add(KeyValueExpression.Parse(ew, expression, caller));
                } else
                    exprs.Add(expression);
            }

            if (exprs.Count == 0 && !argsOptional)
                throw new UnexpectedTokenException($"Was expecting an expression between {left} and {right}");

            ew.Next();
            args.Arguments = exprs.ToArray();
            return args;
        }

        public static ArgumentsExpression Parse(ExpressionWalker ew, Func<TokenMatch, bool> parseTill, bool argsOptional, Type caller = null) {
            var args = new ArgumentsExpression();
            var exprs = new List<Expression>();

            while (!parseTill(ew.Current) && ew.HasNext) {
                if (ew.Current.Token == ExpressionToken.Comma) {
                    if (ew.HasBack && ew.PeakBack.Token == ExpressionToken.Comma) {
                        exprs.Add(NullIdentity.Instance);
                    }

                    ew.NextOrThrow();
                    continue;
                }

                var expression = ParseExpression(ew, caller);
                if (ew.IsCurrent(ExpressionToken.Colon)) {
                    //handle keyvalue item
                    exprs.Add(KeyValueExpression.Parse(ew, expression));
                } else
                    exprs.Add(expression);
            }

            if (exprs.Count == 0 && !argsOptional)
                throw new UnexpectedTokenException($"Was expecting arguments but found none while argsOptional is false");

            ew.Next();
            args.Arguments = exprs.ToArray();
            return args;
        }

        public override IEnumerable<RegexResult> Matches() {
            for (var i = 0; i < Arguments.Length; i++) {
                foreach (var match in Arguments[i].Matches()) {
                    yield return match;
                }

                if (i < Arguments.Length - 1) { //is not last
                    yield return _seperator;
                }
            }
        }

        public override IEnumerable<Expression> Iterate() {
            return this.Yield().Concat(Arguments.SelectMany(e => e.Iterate()));
        }
    }
}