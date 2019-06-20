using System;

namespace Regen.Compiler.Expressions {
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class ExpressionTokenAttribute : Attribute {
        public string Regex { get; }
        public int Order { get; }

        public ExpressionTokenAttribute(string regex, int order) {
            Regex = regex;
            Order = order;
        }
    }
}