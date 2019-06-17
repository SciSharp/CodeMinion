using System;
using System.Diagnostics;
using System.Linq;

namespace Regen.DataTypes {

    [DebuggerDisplay("Number: {" + nameof(Value) + "}")]
    public class NumberScalar : Scalar {
        public override string Emit() {
            return Value.ToString();
        }

        /// <summary>
        ///     Emit the <see cref="Data.Value"/> for expression evaluation purposes.
        /// </summary>
        /// <returns></returns>
        public override string EmitExpressive() {
            var emission = Value.ToString();

            if (!emission.Contains(".")) {
                switch (Value) {
                    case Double @double:
                        emission += ".0d";
                        break;
                    case Single single: 
                        emission += ".0f";
                        break;
                    case Decimal @decimal: 
                        emission += ".0M";
                        break;
                }
            } else if (!char.IsLetter(emission.Last())) {
                switch (Value) {
                    case Double @double:
                        emission += "d";
                        break;
                    case Single single: 
                        emission += "f";
                        break;
                    case Decimal @decimal: 
                        emission += "M";
                        break;
                }
            }

            return emission;
        }

        public T As<T>(T @default) where T : IComparable {
            return Value is T ret ? ret : @default;
        }

        public T Cast<T>() where T : IComparable {
            return (T) Convert.ChangeType(Value, typeof(T));
        }

        public NumberScalar(object value) : base(value) { }
    }
}