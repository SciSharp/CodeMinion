namespace Regen.Compiler.Expressions {
    public class ForLoop {
        public int From { get; set; }
        public int To { get; set; }
        public int Index { get; set; }

        public bool CanNext() => Index < To;

        public int Next() {
            if (CanNext()) {
                var currently = Index;
                Index++;
                return currently;
            }

            return 0;
        }
    }
}