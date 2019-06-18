using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Regen.DataTypes {
    [DebuggerDisplay("Array: {this}")]
    public class Array : Data {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public List<Scalar> Values { get; set; }

        public Array() {
            Values = new List<Scalar>(0);
        }

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

        /// <summary>
        ///     Creates an array with given values that are wrapped using <see cref="Scalar.Create"/>.
        /// </summary>
        /// <param name="objs">Objects that are supported by <see cref="Scalar.Create"/>.</param>
        public static Array Create(params object[] objs) {
            if (objs == null || objs.Length == 0)
                return new Array();
            return new Array(objs.Select(Scalar.Create).ToList());
        }

        /// <summary>
        ///     Creates an array with given values that are wrapped using <see cref="Scalar.Create"/>.
        /// </summary>
        /// <param name="objs">Objects that are supported by <see cref="Scalar.Create"/>.</param>
        public static Array Create(IEnumerable<object> objs) {
            if (objs == null)
                return new Array();
            var objs_ = objs.ToList();
            if (objs_.Count == 0)
                return new Array();

            return new Array(objs_.Select(Scalar.Create).ToList());
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() {
            return Emit();
        }
    }
}