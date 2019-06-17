using System.Diagnostics;

namespace Regen.DataTypes {
    /// <summary>
    ///     A scalar that that represents null or <see cref="void"/>.
    /// </summary>
    [DebuggerDisplay("Value: null")]
    public class NullScalar : Scalar {
        public override string Emit() {
            return "";
        }

        public override string EmitExpressive() {
            return $"null";
        }

        public NullScalar() : base(null) { }
    }
}