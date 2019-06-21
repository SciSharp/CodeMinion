using System.Collections.Generic;
using System.Text.RegularExpressions;
using Regen.Helpers;

namespace Regen.Compiler.Expressions {
    /// <summary>
    ///     Parses: key: value<br></br>
    ///     example: var a = ["hey": 33, 5: 33]
    /// </summary>
    public class KeyValueExpression : Expression {
        private static readonly Match _match = ":".WrapAsMatch();

        public Expression Key;
        public Expression Value;

        public KeyValueExpression(Expression key, Expression value) {
            Key = key;
            Value = value;
        }

        public static KeyValueExpression Parse(ExpressionWalker ew, Expression left = null) {
            var key = left ?? ew.ParseExpression(typeof(KeyValueExpression));
            ew.IsCurrentOrThrow(ExpressionToken.Colon);
            ew.NextOrThrow();
            var value = ew.ParseExpression(typeof(KeyValueExpression));
            return new KeyValueExpression(key, value);
        }

        public override IEnumerable<Match> Matches() {
            foreach (var match in Key.Matches()) {
                yield return match;
            }

            yield return _match;
            foreach (var match in Value.Matches()) {
                yield return match;
            }
        }
    }
}