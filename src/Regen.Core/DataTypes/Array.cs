using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Regen.DataTypes {
    [DebuggerDisplay("Array: {this}")]
    public class Array : Data {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public List<Scalar> Values { get; set; } = new List<Scalar>(0);

        public Array() { }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Array(List<Scalar> values) {
            Values = values;
        }

        public override object Value {
            get => Values;
            set => Values = (List<Scalar>) value;
        }

        public object this[int index] {
            get => Values[index].Value;
            set => Values[index] = Scalar.Create(value);
        }

        public override string Emit() {
            return $"[{string.Join("|", Values.Select(v => v.Emit()))}]";
        }

        /// <summary>
        ///     Emit the <see cref="Data.Value"/> for expression evaluation purposes.
        /// </summary>
        /// <returns></returns>
        public override string EmitExpressive() {
            return $"(new object[]{{{string.Join(",", Values.Select(v => v.EmitExpressive()))}}})";
        }

        public string Emit(int index) {
            return Values[index].Emit();
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() {
            return Emit();
        }
    }
}