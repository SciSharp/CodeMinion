namespace Regen.Flee.Parsing._5.alpha2.PerCederberg.Grammatica.Runtime {
    /**
     * The token match status. This class contains logic to ensure that
     * only the longest match is considered.
     */
    internal class TokenMatch {
        private int _length = 0;
        private TokenPattern _pattern = null;

        public void Clear() {
            _length = 0;
            _pattern = null;
        }

        public int Length => _length;

        public TokenPattern Pattern => _pattern;

        public void Update(int length, TokenPattern pattern) {
            if (this._length < length) {
                this._length = length;
                this._pattern = pattern;
            }
        }
    }
}