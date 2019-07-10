using System;

namespace Regen.Parser.Expressions {
    public class InteractableExpression : Expression {
        //todo this is not the best choice of design, IdentityExpression.Parse does the same, this is just more intuitive. both can co-exist.
        /// <summary>
        ///     In cases like: property(1,2)[0]  we have chained interaction, this resolves if current <see cref="result"/> has more interactions. If so, parses them.
        /// </summary>
        /// <param name="result">The result expression</param>
        /// <param name="ew">The walker with position right after <see cref="result"/></param>
        /// <param name="caller">Internal use.</param>
        /// <returns></returns>
        public static Expression TryExpand(Expression result, ExpressionWalker ew, Type caller = null) {
            if (ew.Current.Token == ExpressionToken.Period) {
                return IdentityExpression.Parse(ew, caller, result);
            }

            if (ew.Current.Token == ExpressionToken.LeftBracet) {
                return IndexerCallExpression.Parse(ew, result);
            }

            if (ew.Current.Token == ExpressionToken.LeftParen) {
                return CallExpression.Parse(ew, result);
            }

            return result;
        }
    }
}