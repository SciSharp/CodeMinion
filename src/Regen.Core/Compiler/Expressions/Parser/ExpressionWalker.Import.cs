using System.Linq;
using Regen.Collections;
using Regen.Helpers;

namespace Regen.Compiler.Expressions {
    public class ImportExpression : Expression {
        /// <summary>
        ///     FullName of the imported type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///     Optional
        /// </summary>
        public string As { get; set; }
    }

    public partial class ExpressionWalker {
        public ImportExpression ParseImport() {
            IsCurrentOrThrow(ExpressionToken.Import);
            NextOrThrow();

            var ret = new ImportExpression();
            ret.Type = TakeForwardWhile(t => t.Token == ExpressionToken.Period || t.Token == ExpressionToken.Literal)
                .Select(t => t.Match.Value).StringJoin();

            if (IsCurrent(ExpressionToken.As)) {
                ret.As = Next(ExpressionToken.Literal).Match.Value;
            }

            return ret;
        }
    }
}