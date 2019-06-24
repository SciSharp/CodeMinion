using System;
using System.Collections.Generic;
using Regen.Helpers;

namespace Regen.Parser.Expressions {
    public class HashtagReferenceExpression : Expression {
        private RegexResult _hashtagMatch;
        private RegexResult _numberMatch;
        public string Number;

        public static Expression Parse(ExpressionWalker ew, Type caller = null) {
            var ret = new HashtagReferenceExpression();

            ew.IsCurrentOrThrow(ExpressionToken.Hashtag);
            ret._hashtagMatch = ew.Current.Match.AsResult();
            ew.NextOrThrow();

            ew.IsCurrentOrThrow(ExpressionToken.NumberLiteral);
            ret._numberMatch = ew.Current.Match.AsResult();
            ret.Number = NumberLiteral.Parse(ew).Value;

            return ret;
        }

        public static Expression TryParse(ExpressionWalker ew, Type caller = null) {
            if (ew.IsCurrent(ExpressionToken.Hashtag) && ew.HasNext && ew.IsNext(ExpressionToken.NumberLiteral)) {
                return Parse(ew, caller);
            }

            return null;
        }

        /// <summary>
        ///     All matches that were used to assemble this expression, in order.
        /// </summary>
        public override IEnumerable<RegexResult> Matches() {
            yield return _hashtagMatch;
            yield return _numberMatch;
        }
    }
}