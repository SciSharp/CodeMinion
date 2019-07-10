﻿using System;
using System.Reflection.Emit;
using Regen.Flee.ExpressionElements.Base.Literals;
using Regen.Flee.InternalTypes;
using Regen.Flee.PublicTypes;

namespace Regen.Flee.ExpressionElements.Literals.Real {
    internal class DoubleLiteralElement : RealLiteralElement {
        private readonly double _myValue;

        private DoubleLiteralElement() { }

        public DoubleLiteralElement(double value) {
            _myValue = value;
        }

        public static DoubleLiteralElement Parse(string image, IServiceProvider services) {
            ExpressionParserOptions options = (ExpressionParserOptions) services.GetService(typeof(ExpressionParserOptions));
            DoubleLiteralElement element = new DoubleLiteralElement();

            try {
                double value = options.ParseDouble(image);
                return new DoubleLiteralElement(value);
            } catch (OverflowException) {
                element.OnParseOverflow(image);
                return null;
            }
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services) {
            ilg.Emit(OpCodes.Ldc_R8, _myValue);
        }

        public override System.Type ResultType => typeof(double);
    }
}