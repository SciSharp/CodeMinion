namespace Regen.Compiler.Helpers {
    /// <summary>
    ///     Represents packed arguments that are to be unpacked when value is to be used.
    /// </summary>
    public class PackedArguments {
        public object[] Objects { get; set; }

        public PackedArguments(params object[] objects) {
            Objects = objects;
        }


        public static implicit operator PackedArguments(object[] objs) {
            return new PackedArguments(objs);
        }
    }
}