using System;
using System.Linq;
using Regen.Exceptions;

namespace Regen.Compiler.Expressions {
    public partial class ExpressionWalker {
        /// <summary>
        ///     Moves next and returns the new current token.
        /// </summary>
        public EToken NextToken() {
            var ret = Next() ? Current : null;
            if (ret == null)
                throw new UnexpectedTokenException<ExpressionToken>(Current);
            return ret;
        }

        public void NextOrThrow() {
            var current = Next() ? Current : null;
            if (current == null)
                throw new UnexpectedTokenException<ExpressionToken>(Current);
        }

        public void NextOrThrow(ExpressionToken tkn) {
            if (!HasNext) {
                throw new UnexpectedEndOfScriptException();
            }

            if (PeakNext.Token != tkn) {
                throw new UnexpectedTokenException<ExpressionToken>(PeakNext);
            }

            var current = Next() ? Current : null;
            if (current == null)
                throw new UnexpectedTokenException<ExpressionToken>(Current);
        }


        public EToken PeakNextOrThrow() {
            var current = HasNext ? PeakNext : null;
            if (current == null)
                throw new UnexpectedTokenException<ExpressionToken>(Current);
            return current;
        }

        public EToken PeakNextOrThrow(ExpressionToken tkn) {
            if (!HasNext) {
                throw new UnexpectedTokenException<ExpressionToken>(Current); //todo change to ENdOfToken
            }

            if (PeakNext.Token != tkn) {
                throw new UnexpectedTokenException<ExpressionToken>(PeakNext);
            }

            var current = HasNext ? PeakNext : null;
            if (current == null)
                throw new UnexpectedTokenException<ExpressionToken>(Current);
            return current;
        }

        public bool NextOut(bool @throw, out EToken tkn) {
            var ret = tkn = Next() ? Current : null;
            if (ret == null && @throw)
                throw new UnexpectedTokenException<ExpressionToken>(Current);
            return ret == null;
        }

        /// <summary>
        ///     Checks if next is <see cref="tkn"/>, if so moves forward. otherwise returns false.
        /// </summary>
        /// <param name="tkn"></param>
        /// <returns></returns>
        public bool OptionalNext(ExpressionToken tkn) {
            return HasNext && PeakNext.Token == tkn && Next();
        }

        /// <summary>
        ///     Checks if next is <see cref="tkn"/>, if so moves forward and returns value. otherwise returns null or throws.
        /// </summary>
        /// <param name="tkn"></param>
        /// <returns></returns>
        public EToken Next(ExpressionToken tkn, bool throwOnFalse = false) {
            if (HasNext && PeakNext.Token == tkn && Next())
                return Current;

            if (throwOnFalse)
                throw new UnexpectedTokenException<ExpressionToken>(Current);
            return null;
        }

        public bool IsCurrent(ExpressionToken tkn) {
            return Current.Token == tkn;
        }

        public bool IsCurrentOrThrow(ExpressionToken tkn) {
            return Current.Token == tkn ? true : throw new InvalidTokenException(tkn, Current.Token);
        }
        public bool IsCurrentAnyOf(params ExpressionToken[] tkns) {
            return tkns.Contains(Current.Token);
        }

        public bool IsCurrentAnyOfOrThrow(params ExpressionToken[] tkns) {
            return tkns.Contains(Current.Token) ? true : throw new InvalidTokenException(tkns[0], Current.Token);
        }


        /// <summary>
        ///     Checks if the <paramref name="obj"/> is next
        /// </summary>
        /// <param name="obj">The token expected</param>
        /// <param name="goBackIfTrue">Go back if it turns to be true</param>
        /// <param name="gotNextIfTrue">Go forward if it turns to be true</param>
        /// <returns></returns>
        public bool IsNext(ExpressionToken obj, bool goBackIfTrue = false, bool gotNextIfTrue = false) {
            if (!HasNext)
                return false;

            if (Equals(obj, PeakNext.Token)) {
                walkOnce(goBackIfTrue, gotNextIfTrue);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Checks if the <paramref name="obj"/> is behind
        /// </summary>
        /// <param name="obj">The token expected</param>
        /// <param name="goBackIfTrue">Go back if it turns to be true</param>
        /// <param name="gotNextIfTrue">Go forward if it turns to be true</param>
        /// <returns></returns>
        public bool IsBack(ExpressionToken obj, bool goBackIfTrue = false, bool gotNextIfTrue = false) {
            if (!HasBack)
                return false;

            if (Equals(obj, PeakBack.Token)) {
                walkOnce(goBackIfTrue, gotNextIfTrue);
                return true;
            }

            return false;
        }

        public bool AreNext(params ExpressionToken[] nexts) {
            using (CheckPoint()) {
                return SkipNext(nexts);
            }
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="nexts"></param>
        /// <returns></returns>
        public bool AreBack(params ExpressionToken[] nexts) {
            using (CheckPoint()) {
                return SkipBack(nexts);
            }
        }

        /// <summary>
        ///     Skip give elements in expected order.
        /// </summary>
        /// <param name="nexts"></param>
        /// <returns>If successfully skipped all given types by order</returns>
        public bool SkipNext(params ExpressionToken[] nexts) {
            foreach (var expected in nexts) {
                if (!Next())
                    return false;
                if (!Equals(Current.Token, expected))
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Skip give elements in expected order.
        /// </summary>
        /// <param name="nexts"></param>
        /// <returns>If successfully skipped all given types by order</returns>
        public bool SkipBack(params ExpressionToken[] nexts) {
            foreach (var expected in nexts) {
                if (!Back())
                    return false;
                if (!Equals(Current.Token, expected))
                    return false;
            }

            return true;
        }
    }
}