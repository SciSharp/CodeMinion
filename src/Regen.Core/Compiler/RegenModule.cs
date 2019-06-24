namespace Regen.Compiler {
    /// <summary>
    ///     Represents a module that is accessible inside an expression. see remarks.
    /// </summary>
    /// <remarks>Example usage: %(<see cref="Name"/>.<see cref="Instance"/>Method(123))</remarks>
    public class RegenModule {
        /// <summary>
        ///     The module name that will later can be accessed %(name.method(123))
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The module object that will be used to import functions into <see cref="Flee"/>.
        /// </summary>
        /// <remarks>All public methods will be imported, including static ones.<br></br>Void methods by default return object null.</remarks>
        public object Instance { get; set; }

        public RegenModule(string name, object instance) {
            Name = name;
            Instance = instance;
        }
    }
}