using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Regen.DataTypes {
    [DebuggerDisplay("String: {" + nameof(Value) + "}")]
    public class StringScalar : Scalar {
        public override string Emit() {
            return (string) Value;
        }

        /// <summary>
        ///     Emit the <see cref="Data.Value"/> for expression evaluation purposes.
        /// </summary>
        /// <returns></returns>
        public override string EmitExpressive() {
            return $"\"{Value}\"";
        }

        /// <summary>
        ///     Converts all character to string.
        /// </summary>
        /// <returns></returns>
        public Array ToArray() {
            return new Array(new List<Scalar>(((string)Value).ToCharArray().Select(c=>new StringScalar(c.ToString()))));
        }

        public StringScalar(object value) : base(value) { }
    }
}