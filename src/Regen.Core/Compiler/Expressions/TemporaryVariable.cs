using System;
using System.Linq;
using Flee.PublicTypes;

namespace Regen.Compiler.Expressions {
    public class TemporaryVariable : IDisposable {
        private static string _uniqueName => "__" + new string(Guid.NewGuid().ToString("N").SkipWhile(char.IsDigit).ToArray());
        private readonly ExpressionContext _ctx;
        private bool _isPerma;
        public object Value { get; set; }
        public string Name { get; set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TemporaryVariable(ExpressionContext ctx, object value, string name) {
            _ctx = ctx;
            Value = value;
            Name = name;
            Store(Name, Value);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TemporaryVariable(ExpressionContext ctx, object value) {
            _ctx = ctx;
            Value = value;
            Name = _uniqueName;
            Store(Name, Value);
        }

        private void Store(string name, object val) {
            if (_ctx.Variables.ContainsKey(name))
                throw new InvalidOperationException($"Variable named {name} is already stored.");
            _ctx.Variables[name] = val;
        }

        public void MarkPermanent() {
            _isPerma = true;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() {
            if (_isPerma)
                return;
            _ctx.Variables.Remove(Name);
        }
    }
}