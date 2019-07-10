using System.Collections.Generic;
using System.Linq;
using Regen.Helpers;

namespace Regen.Parser.Expressions {
    public class PropertyIdentity : Identity {
        private static readonly RegexResult _matchPeriod = ".".AsResult();
        public Expression Left { get; set; }
        public Expression Right { get; set; }

        public PropertyIdentity(Expression left, Expression right) {
            Left = left;
            Right = right;
        }

        public override IEnumerable<RegexResult> Matches() {
            foreach (var match in Left.Matches()) {
                yield return match;
            }

            yield return _matchPeriod;

            foreach (var match in Right.Matches()) {
                yield return match;
            }
        }

        public override IEnumerable<Expression> Iterate() {
            return this.Yield().Concat(Left.Iterate()).Concat(Right.Iterate());
        }
    }
}