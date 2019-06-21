using System;
using System.Diagnostics.Eventing.Reader;

namespace Regen.Compiler.Expressions {
    public partial class ExpressionWalker {
        public Expression ParseExpression(Type caller = null) {
            Expression ret = null;
            var current = Current.Token;
            if (current == ExpressionToken.Literal) {
                var peak = PeakNextOrThrow().Token;
                if (peak == ExpressionToken.LeftParen) {
                    ret = CallExpression.Parse(this);
                } else if (peak == ExpressionToken.Period && caller != typeof(IdentityExpression)) {
                    ret = IdentityExpression.Parse(this);
                } else if (peak == ExpressionToken.LeftBracet) {
                    ret = IndexerCallExpression.Parse(this);
                } else if (peak == ExpressionToken.New) {
                    ret = NewExpression.Parse(this);
                } else if (RightOperatorExpression.IsNextAnRightUniOperation(this, caller) && caller != typeof(RightOperatorExpression)) {
                    ret = RightOperatorExpression.Parse(this);
                } else if (OperatorExpression.IsNextAnOperation(this) && caller != typeof(OperatorExpression) && caller != typeof(RightOperatorExpression)) {
                    ret = OperatorExpression.Parse(this);
                } else {
                    ret = IdentityExpression.Parse(this, caller);
                }
            } else if (LeftOperatorExpression.IsCurrentAnLeftUniOperation(this)) {
                ret = LeftOperatorExpression.Parse(this);
            } else if (current == ExpressionToken.NumberLiteral) {
                ret = NumberLiteral.Parse(this);
            } else if (current == ExpressionToken.StringLiteral) {
                ret = StringLiteral.Parse(this);
            } else if (current == ExpressionToken.LeftParen) {
                ret = GroupExpression.Parse(this, ExpressionToken.LeftParen, ExpressionToken.RightParen);
            } else if (current == ExpressionToken.LeftBracet) {
                ret = ArrayExpression.Parse(this);
            } else if (current == ExpressionToken.New) {
                ret = NewExpression.Parse(this);
            } else if (current == ExpressionToken.Boolean) {
                ret = BooleanLiteral.Parse(this);
            } else if (current == ExpressionToken.CharLiteral) {
                ret = CharLiteral.Parse(this);
            } else if (current == ExpressionToken.Throw) {
                ret = ThrowExpression.Parse(this);
            } else {
                throw new Exception($"Token was expected to be an expression but got {this.Current.Token}");
            }

            //here we find trailing expressions 
            while (OperatorExpression.IsCurrentAnOperation(this)) {
                if (RightOperatorExpression.IsCurrentAnRightUniOperation(this) && caller != typeof(OperatorExpression)) {
                    ret = RightOperatorExpression.Parse(this, ret);
                } else if (OperatorExpression.IsCurrentAnOperation(this)) {
                    ret = OperatorExpression.Parse(this, ret);
                }
            }

            return ret;
        }
    }
}