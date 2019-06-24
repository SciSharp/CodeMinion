using System;

namespace Regen.Parser {
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class ExpressionTokenAttribute : Attribute {
        public string Regex { get; }
        public int Order { get; }
        public string Emit { get; set; }
        public ExpressionTokenAttribute(string regex, int order, string emit) {
            Regex = regex;
            Order = order;
            Emit = emit;
        }
    }
}