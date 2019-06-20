using System;
using System.Collections.Generic;

namespace Regen.Compiler.Expressions {
    public class ArgumentsExpression : Expression {
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
                        exprs.Add(NullExpression.Instance);
                    }

                    ew.NextOrThrow();
                    continue;
                }

                exprs.Add(ew.ParseExpression());
            }

            if (exprs.Count == 0 && !argsOptional)
                throw new Exception($"Was expecting an expression between {left} and {right}");

            ew.Next();
            args.Arguments = exprs.ToArray();
            return args;
        }
    }
}