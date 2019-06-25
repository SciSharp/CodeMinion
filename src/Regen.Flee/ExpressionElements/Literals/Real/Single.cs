﻿using System;
using System.Reflection.Emit;
using Regen.Flee.ExpressionElements.Base.Literals;
using Regen.Flee.InternalTypes;
using Regen.Flee.PublicTypes;

namespace Regen.Flee.ExpressionElements.Literals.Real {
    internal class SingleLiteralElement : RealLiteralElement {
        private readonly float _myValue;

        private SingleLiteralElement() { }

        public SingleLiteralElement(float value) {
            _myValue = value;
        }

        public static SingleLiteralElement Parse(string image, IServiceProvider services) {
            ExpressionParserOptions options = (ExpressionParserOptions) services.GetService(typeof(ExpressionParserOptions));
            SingleLiteralElement element = new SingleLiteralElement();

            try {
                float value = options.ParseSingle(image);
                return new SingleLiteralElement(value);
            } catch (OverflowException) {
                element.OnParseOverflow(image);
                return null;
            }
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services) {
            ilg.Emit(OpCodes.Ldc_R4, _myValue);
        }

        public override System.Type ResultType => typeof(float);
    }
}