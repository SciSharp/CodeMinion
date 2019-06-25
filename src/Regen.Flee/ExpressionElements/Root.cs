using System;
using System.Reflection.Emit;
using Regen.Flee.ExpressionElements.Base;
using Regen.Flee.InternalTypes;
using Regen.Flee.PublicTypes;
using Regen.Flee.Resources;

namespace Regen.Flee.ExpressionElements {
    internal class RootExpressionElement : ExpressionElement {
        private readonly ExpressionElement _myChild;
        private readonly Type _myResultType;

        public RootExpressionElement(ExpressionElement child, Type resultType) {
            _myChild = child;
            _myResultType = resultType;
            this.Validate();
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services) {
            _myChild.Emit(ilg, services);
            ImplicitConverter.EmitImplicitConvert(_myChild.ResultType, _myResultType, ilg);

            ExpressionOptions options = (ExpressionOptions) services.GetService(typeof(ExpressionOptions));

            if (options.IsGeneric == false) {
                ImplicitConverter.EmitImplicitConvert(_myResultType, typeof(object), ilg);
            }

            ilg.Emit(OpCodes.Ret);
        }

        private void Validate() {
            if (ImplicitConverter.EmitImplicitConvert(_myChild.ResultType, _myResultType, null) == false) {
                base.ThrowCompileException(CompileErrorResourceKeys.CannotConvertTypeToExpressionResult, CompileExceptionReason.TypeMismatch, _myChild.ResultType.Name, _myResultType.Name);
            }
        }

        public override System.Type ResultType => typeof(object);
    }
}