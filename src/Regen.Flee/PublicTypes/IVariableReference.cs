namespace Regen.Flee.PublicTypes {
    /// <summary>
    ///     Represents an object that is a placeholder and is refering to an other variable.
    /// </summary>
    public interface IVariableReference {
        /// <summary>
        ///     The name of the targeted variable.
        /// </summary>
        string Target { get; }
    }
}