﻿using System;
using System.Reflection;
using System.Reflection.Emit;
using Regen.Flee.ExpressionElements.Base;
using Regen.Flee.InternalTypes;

namespace Regen.Flee.ExpressionElements {
    internal class NegateElement : UnaryElement {
        public NegateElement() { }

        protected override System.Type GetResultType(System.Type childType) {
            TypeCode tc = Type.GetTypeCode(childType);

            MethodInfo mi = Utility.GetSimpleOverloadedOperator("UnaryNegation", childType, null);
            if ((mi != null)) {
                return mi.ReturnType;
            }

            switch (tc) {
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return childType;
                case TypeCode.UInt32:
                    return typeof(Int64);
                default:
                    return null;
            }
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services) {
            Type resultType = this.ResultType;
            MyChild.Emit(ilg, services);
            ImplicitConverter.EmitImplicitConvert(MyChild.ResultType, resultType, ilg);

            MethodInfo mi = Utility.GetSimpleOverloadedOperator("UnaryNegation", resultType, null);

            if (mi == null) {
                ilg.Emit(OpCodes.Neg);
            } else {
                ilg.Emit(OpCodes.Call, mi);
            }
        }
    }
}