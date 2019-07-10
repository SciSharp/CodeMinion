using System.Collections.Generic;
using System.Linq;
using Regen.Helpers;

namespace Regen.Parser.Expressions {
    public class ImportExpression : Expression {
        private static readonly RegexResult _matchImport = "import".AsResult();
        private static readonly RegexResult _matchAs = "as".AsResult();
        private RegexResult[] _matchType;
        private RegexResult _matchAlias;

        /// <summary>
        ///     FullName of the imported type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///     Optional, can be null.
        /// </summary>
        public string As { get; set; }

        public override IEnumerable<RegexResult> Matches() {
            yield return _matchImport;
            yield return _matchWhitespace;
            foreach (var match in _matchType) {
                yield return match;
            }

            if (_matchAlias == null)
                yield break;

            yield return _matchWhitespace;
            yield return _matchAs;
            yield return _matchWhitespace;
            yield return _matchAlias;
        }

        public static ImportExpression Parse(ExpressionWalker ew) {
            ew.IsCurrentOrThrow(ExpressionToken.Import);
            ew.NextOrThrow();

            // ReSharper disable once UseObjectOrCollectionInitializer
            var ret = new ImportExpression();
            if (ew.IsCurrent(ExpressionToken.StringLiteral)) {
                ret._matchType = new RegexResult[] {ew.Current.Match.AsResult()};
                ret.Type = ew.Current.Match.Value.Trim('\"');
            } else {
                ret._matchType = ew.TakeForwardWhile(t => t.Token == ExpressionToken.Period || t.Token == ExpressionToken.Literal)
                    .Select(t => t.Match.AsResult()).ToArray();

                ret.Type = ret._matchType.Select(m => m.Value).StringJoin();

                if (ew.IsCurrent(ExpressionToken.As)) {
                    ret._matchAlias = ew.Next(ExpressionToken.Literal, true).Match.AsResult();
                    ret.As = ret._matchAlias.Value;
                }
            }

            ew.Next();
            return ret;
        }
    }
}