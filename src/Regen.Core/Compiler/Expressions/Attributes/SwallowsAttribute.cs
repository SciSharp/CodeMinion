using System;

namespace Regen.Compiler.Expressions {
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class SwallowsAttribute : Attribute {
        public ExpressionToken[] Targets { get; }

        public SwallowsAttribute(params ExpressionToken[] targets) {
            Targets = targets;
        }
    }
}