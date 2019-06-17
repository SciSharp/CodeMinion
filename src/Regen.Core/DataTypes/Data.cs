using System.Diagnostics;

namespace Regen.DataTypes {
    /// <summary>
    ///     Base data type, all stored data types (e.g. <see cref="StringScalar"/>, <see cref="NullScalar"/>) inherit this.
    /// </summary>
    public abstract class Data {
        public abstract object Value { get; set; }

        /// <summary>
        ///     Emit the <see cref="Value"/> for generation purposes.
        /// </summary>
        /// <returns></returns>
        public abstract string Emit();

        /// <summary>
        ///     Emit the <see cref="Value"/> for expression evaluation purposes.
        /// </summary>
        /// <returns></returns>
        public abstract string EmitExpressive();

    }
}