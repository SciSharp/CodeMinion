using Regen.DataTypes;

namespace Regen.Compiler {
    public interface IRegenModule {
        /// <summary>
        ///     Simply implement as return this
        /// </summary>
        /// <remarks>Because <see cref="Flee"/> implements external modules as a namespace and not an actual type, returns self </remarks>
        Data Self();
    }
}