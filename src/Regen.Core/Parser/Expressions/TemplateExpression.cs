using System.Collections.Generic;
using System.Linq;
using Regen.Helpers;

namespace Regen.Parser.Expressions {
    public class TemplateExpression : Expression {
        private static readonly RegexResult _matchTemplate = "template".AsResult();
        private static readonly RegexResult _forEvery = "for every".AsResult();
        private RegexResult[] _matchPath;
        private RegexResult _matchForEvery;
        private RegexResult _expressionResult;

        /// <summary>
        ///     FullName of the imported type.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///     Optional, can be null.
        /// </summary>
        public ArgumentsExpression Arguments;

        public override IEnumerable<RegexResult> Matches() {
            yield return _matchTemplate;
            yield return _matchWhitespace;
            foreach (var match in _matchPath) {
                yield return match;
            }

            yield return _matchWhitespace;
            yield return _forEvery;
            yield return _matchWhitespace;
            foreach (var match in Arguments.Matches())
                yield return match;
        }

        public static TemplateExpression Parse(ExpressionWalker ew) {
            ew.IsCurrentOrThrow(ExpressionToken.Template);
            ew.NextOrThrow();

            // ReSharper disable once UseObjectOrCollectionInitializer
            var ret = new TemplateExpression();
            ew.IsCurrentOrThrow(ExpressionToken.StringLiteral);

            ret._matchPath = new RegexResult[] {ew.Current.Match.AsResult()};
            ret.Path = ew.Current.Match.Value.StartsWith("\"") ? ew.Current.Match.Value.Substring(1, ew.Current.Match.Length-2) : ew.Current.Match.Value;

            ret._matchForEvery = ew.Next(ExpressionToken.ForEvery, true).Match.AsResult();
            ew.NextOrThrow();
            ret.Arguments = ArgumentsExpression.Parse(ew, token => token.Token == ExpressionToken.NewLine || token.Token == ExpressionToken.Mod, false, typeof(ForeachExpression));
            //we have to fall back because ArgumentsExpression.Parse swallows mod.
            ew.IsBack(ExpressionToken.Mod, true);
            if (ew.IsCurrent(ExpressionToken.Mod))
                ew.Back(1);

            return ret;
        }
    }
}