using System;
using System.Reflection.Emit;
using Regen.Flee.ExpressionElements.Base.Literals;
using Regen.Flee.InternalTypes;

namespace Regen.Flee.ExpressionElements.Literals {
    internal class NullLiteralElement : LiteralElement {
        public override void Emit(FleeILGenerator ilg, IServiceProvider services) {
            ilg.Emit(OpCodes.Ldnull);
        }

        public override System.Type ResultType => typeof(Null);
    }
}