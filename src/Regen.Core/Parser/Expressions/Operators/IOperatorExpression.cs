using System.Diagnostics;

namespace Regen.Compiler.Expressions {
    public interface IOperatorExpression {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ExpressionToken Op { get; set; }
    }
}