using Regen.Compiler.Helpers;

namespace Regen.Parser.Expressions {
    public class ForeachExpression : Expression {
        public ArgumentsExpression Arguments;
        public StringSlice Content;
    }
}