using System.Diagnostics;

namespace Regen.Parser.Expressions {
    public interface IOperatorExpression {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ExpressionToken Op { get; set; }
    }
}