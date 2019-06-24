using Regen.Compiler.Helpers;

namespace Regen.Compiler.Expressions {
    public class ForeachExpression : Expression {
        public ArgumentsExpression Arguments;
        public StringSlice Content;
    }
}