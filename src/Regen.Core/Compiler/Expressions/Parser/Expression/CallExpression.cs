namespace Regen.Compiler.Expressions {
    public class CallExpression : Expression {
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
    }
}