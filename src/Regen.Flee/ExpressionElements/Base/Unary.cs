using System;
using Regen.Flee.PublicTypes;
using Regen.Flee.Resources;

namespace Regen.Flee.ExpressionElements.Base {
    internal abstract class UnaryElement : ExpressionElement {
        protected ExpressionElement MyChild;

        private Type _myResultType;

        public void SetChild(ExpressionElement child) {
            MyChild = child;
            _myResultType = this.GetResultType(child.ResultType);

            if (_myResultType == null) {
                base.ThrowCompileException(CompileErrorResourceKeys.OperationNotDefinedForType, CompileExceptionReason.TypeMismatch, MyChild.ResultType.Name);
            }
        }

        protected abstract Type GetResultType(Type childType);

        public override System.Type ResultType => _myResultType;
    }
}