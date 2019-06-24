using System;
using System.Collections.Generic;
using System.Linq;
using Regen.Helpers;

namespace Regen.Parser.Expressions {
    /// <summary>
    ///     Parses: key: value<br></br>
    ///     example: var a = ["hey": 33, 5: 33]
    /// </summary>
    public class KeyValueExpression : Expression {
        private static readonly RegexResult _match = ":".AsResult();

        public Expression Key;
        public Expression Value;

        public KeyValueExpression(Expression key, Expression value) {
            Key = key;
            Value = value;
        }

        public static KeyValueExpression Parse(ExpressionWalker ew, Expression left = null, Type caller = null) {
            var key = left ?? ParseExpression(ew, caller ?? typeof(KeyValueExpression));
            ew.IsCurrentOrThrow(ExpressionToken.Colon);
            ew.NextOrThrow();
            var value = ParseExpression(ew, caller ?? typeof(KeyValueExpression));
            return new KeyValueExpression(key, value);
        }

        public override IEnumerable<RegexResult> Matches() {
            foreach (var match in Key.Matches()) {
                yield return match;
            }

            yield return _match;
            foreach (var match in Value.Matches()) {
                yield return match;
            }
        }

        public override IEnumerable<Expression> Iterate() {
            return this.Yield().Concat(Key.Iterate()).Concat(Value.Iterate());
        }
    }
}