using System;
using Regen.Flee.ExpressionElements.Base.Literals;
using Regen.Flee.InternalTypes;

namespace Regen.Flee.ExpressionElements.Literals.Integral {
    internal class UInt64LiteralElement : IntegralLiteralElement {
        private readonly UInt64 _myValue;

        public UInt64LiteralElement(string image, System.Globalization.NumberStyles ns) {
            try {
                _myValue = UInt64.Parse(image, ns);
            } catch (OverflowException) {
                base.OnParseOverflow(image);
            }
        }

        public UInt64LiteralElement(UInt64 value) {
            _myValue = value;
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services) {
            EmitLoad(Convert.ToInt64(_myValue), ilg);
        }

        public override System.Type ResultType => typeof(UInt64);
    }
}