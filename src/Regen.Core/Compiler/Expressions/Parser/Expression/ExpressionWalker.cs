using System;

namespace Regen.Compiler.Expressions {
    public partial class ExpressionWalker {
        public Expression ParseExpression(Type caller = null) {
            var current = Current.Token;
            if (current == ExpressionToken.Literal) {
                var peak = PeakNextOrThrow().Token;
                if (peak == ExpressionToken.LeftParen) {
                    return CallExpression.Parse(this);
                } else if (peak == ExpressionToken.Period && caller != typeof(IdentityExpression)) {
                    return IdentityExpression.Parse(this);
                } else if (peak == ExpressionToken.LeftBracet) {
                    return IndexerCallExpression.Parse(this);
                } else if (peak == ExpressionToken.New) {
                    return NewExpression.Parse(this);
                } else if (OperatorExpression.IsNextAnOperation(this) && caller != typeof(OperatorExpression)) {
                    return OperatorExpression.Parse(this);
                } else {
                    return IdentityExpression.Parse(this, caller);
                }
            } else if (OperatorExpression.IsNextAnOperation(this) && caller != typeof(OperatorExpression)) {
                return OperatorExpression.Parse(this);
            } else if (current == ExpressionToken.NumberLiteral) {
                return NumberLiteral.Parse(this);
            } else if (current == ExpressionToken.StringLiteral) {
                return StringLiteral.Parse(this);
            } else if (current == ExpressionToken.LeftParen) {
                return GroupExpression.Parse(this, ExpressionToken.LeftParen, ExpressionToken.RightParen);
            } else if (current == ExpressionToken.LeftBracet) {
                return ArrayExpression.Parse(this);
            } else if (current == ExpressionToken.New) {
                return NewExpression.Parse(this);
            } else if (current == ExpressionToken.Boolean) {
                return BooleanLiteral.Parse(this);
            } else if (current == ExpressionToken.CharLiteral) {
                return CharLiteral.Parse(this);
            } else if (current == ExpressionToken.Throw) {
                return ThrowExpression.Parse(this);
            }

            return new Expression();
        }
    }
}