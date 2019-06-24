using System;
using System.Collections.Generic;

namespace Regen.Helpers.Collections {
    public class TokenWalker<T> : ListWalker<T> {
        public TokenWalker(IList<T> list) : base(list) { }
        protected TokenWalker() { }

        /// <summary>
        ///     Checks if the <paramref name="obj"/> is next
        /// </summary>
        /// <param name="obj">The token expected</param>
        /// <param name="goBackIfTrue">Go back if it turns to be true</param>
        /// <param name="gotNextIfTrue">Go forward if it turns to be true</param>
        /// <returns></returns>
        public bool IsNext(T obj, bool goBackIfTrue = false, bool gotNextIfTrue = false) {
            if (!HasNext)
                return false;

            if (Equals(obj, PeakNext)) {
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
        public bool IsBack(T obj, bool goBackIfTrue = false, bool gotNextIfTrue = false) {
            if (!HasBack)
                return false;

            if (Equals(obj, PeakBack)) {
                walkOnce(goBackIfTrue, gotNextIfTrue);
                return true;
            }

            return false;
        }

        public bool AreNext(params T[] nexts) {
            using (CheckPoint()) {
                return SkipNext(nexts);
            }
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="nexts"></param>
        /// <returns></returns>
        public bool AreBack(params T[] nexts) {
            using (CheckPoint()) {
                return SkipBack(nexts);
            }
        }

        /// <summary>
        ///     Skip give elements in expected order.
        /// </summary>
        /// <param name="nexts"></param>
        /// <returns>If successfully skipped all given types by order</returns>
        public bool SkipNext(params T[] nexts) {
            foreach (var expected in nexts) {
                if (!Next())
                    return false;
                if (!Equals(Current, expected))
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Skip give elements in expected order.
        /// </summary>
        /// <param name="nexts"></param>
        /// <returns>If successfully skipped all given types by order</returns>
        public bool SkipBack(params T[] nexts) {
            foreach (var expected in nexts) {
                if (!Back())
                    return false;
                if (!Equals(Current, expected))
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Enters a checkpoint, applies cursor change only if <see cref="successFunc"/> return true.
        /// </summary>
        /// <param name="successFunc">Return true to apply changes and false to revert them</param>
        /// <returns>The result from <see cref="successFunc"/></returns>
        public bool ApplyOnlyIfTrue(Func<bool> successFunc) {
            bool ret = false;
            int index = Cursor;
            try {
                using (CheckPoint()) {
                    ret = successFunc();
                    index = Cursor;
                    return ret;
                }
            } finally {
                if (ret)
                    this.Goto(index);
            }

            ;
        }

        /// <summary>
        ///     Enters a checkpoint, applies cursor change only if <see cref="successFunc"/> return false.
        /// </summary>
        /// <param name="successFunc">Return true to revert changes and false to apply them</param>
        /// <returns>The result from <see cref="successFunc"/></returns>
        public bool ApplyOnlyIfFalse(Func<bool> successFunc) {
            bool ret = false;
            int index = Cursor;
            try {
                using (CheckPoint()) {
                    ret = successFunc();
                    index = Cursor;
                    return ret;
                }
            } finally {
                if (!ret)
                    this.Goto(index);
            }

            ;
        }


        protected void walkOnce(bool goBackIfTrue, bool gotNextIfTrue) {
            if ((gotNextIfTrue && goBackIfTrue) || (!gotNextIfTrue && !goBackIfTrue))
                return;
            if (gotNextIfTrue) {
                Next();
                return;
            }

            if (goBackIfTrue) {
                Back();
                return;
            }
        }
    }

    public static class TokenWalker {
        #region Static

        /// <summary>
        ///     Wraps <paramref name="list"/> with <see cref="TokenWalker{T}"/>
        /// </summary>
        /// <typeparam name="TT"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static TokenWalker<TT> WrapWalker<TT>(IList<TT> list) {
            return new TokenWalker<TT>(list);
        }

        /// <summary>
        ///     Wraps <paramref name="list"/> with <see cref="TokenWalker{T}"/>
        /// </summary>
        /// <typeparam name="TT"></typeparam>
        /// <param name="list"></param>
        /// <param name="setCursorAt">At what index/cursor should the walker start</param>
        /// <returns></returns>
        public static TokenWalker<TT> WrapWalker<TT>(IList<TT> list, int setCursorAt) {
            return new TokenWalker<TT>(list) {Cursor = setCursorAt};
        }

        #endregion
    }
}