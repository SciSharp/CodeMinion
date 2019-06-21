using System.Collections.Generic;
using System.Text.RegularExpressions;
using Regen.Helpers;

namespace Regen.Compiler.Expressions {
    public class PropertyIdentity : Identity {
        public Expression Left { get; set; }
        public Expression Right { get; set; }

        public PropertyIdentity(Expression left, Expression right) {
            Left = left;
            Right = right;
        }

        public override IEnumerable<Match> Matches() {
            foreach (var match in Left.Matches()) {
                yield return match;
            }

            yield return ".".WrapAsMatch();

            foreach (var match in Right.Matches()) {
                yield return match;
            }
        }
    }
}