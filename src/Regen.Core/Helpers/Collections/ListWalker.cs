using System;
using System.Collections.Generic;
using System.Linq;

namespace Regen.Helpers.Collections {
    public struct IndexedItem<T> {
        public T Value;
        public int Index;
    }

    /// <summary>
    ///     Wraps result in the item with it's index - not that this makes a copy of the chosen list.
    /// </summary>
    public class IndexedListWalker<T> : ListWalker<IndexedItem<T>> {
        /// <inheritdoc />
        public IndexedListWalker(IList<IndexedItem<T>> list) : base(list) { }

        public IndexedListWalker(IList<T> list) : base(ZipIndex(list).Select(zi => new IndexedItem<T>() {Index = zi.Index, Value = zi.Value}).ToList()) { }

        /// <inheritdoc />
        protected IndexedListWalker() { }

        private static IEnumerable<(int Index, T Value)> ZipIndex(IEnumerable<T> enumerable) {
            return enumerable.Select((obj, i) => (Index: i, Value: obj));
        }
    }

    public static class ListWalker {
        #region Static

        /// <summary>
        ///     Wraps <paramref name="list"/> with <see cref="ListWalker{T}"/>
        /// </summary>
        /// <typeparam name="TT"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static ListWalker<TT> WrapWalker<TT>(this IList<TT> list) {
            return new ListWalker<TT>(list);
        }

        /// <summary>
        ///     Wraps <paramref name="list"/> with <see cref="ListWalker{T}"/>
        /// </summary>
        /// <typeparam name="TT"></typeparam>
        /// <param name="list"></param>
        /// <param name="setCursorAt">At what index/cursor should the walker start</param>
        /// <returns></returns>
        public static ListWalker<TT> WrapWalker<TT>(this IList<TT> list, int setCursorAt) {
            return new ListWalker<TT>(list) {Cursor = setCursorAt};
        }

        /// <summary>
        ///     Creates a linked <see cref="IDisposable"/> that replicated <see cref="IListWalker.CheckPoint"/> for every <paramref name="walkers"/> passed.
        /// </summary>
        /// <param name="walkers">the walkers to simutaniously enter checkpoint and exit on dispose</param>
        /// <returns>IDisposable that upon dispose causes exiting from checkpoint.</returns>
        public static IDisposable Checkpoint(params IListWalker[] walkers) {
            return new GroupDispose(walkers.Select(w => w.CheckPoint()));
        }

        private class GroupDispose : IDisposable {
            private IDisposable[] _dises;

            public GroupDispose(IEnumerable<IDisposable> dises) {
                _dises = dises.ToArray();
            }

            /// <inheritdoc />
            public void Dispose() {
                foreach (var disposable in _dises) {
                    disposable.Dispose();
                }

                _dises = null;
            }
        }

        #endregion
    }

    /// <summary>
    ///     Allows traveling an array, take, skipping, checkpoint system and more.
    /// </summary>
    /// <typeparam name="T">The element type of the array.</typeparam>
    public class ListWalker<T> : ICloneable, IEquatable<ListWalker<T>>, IComparable<ListWalker<T>>, IComparable, IListWalker {
        /// <summary>
        ///     All checkpoints that are currently saved.
        /// </summary>
        public Dictionary<string, int> Checkpoints { get; } = new Dictionary<string, int>();

        /// <summary>
        ///     The <see cref="IList{T}"/> we are currently walking.
        /// </summary>
        public IList<T> Walking { get; set; }

        /// <summary>
        ///     The index the walker is currently at. Note: 0 based.
        /// </summary>
        public int Cursor { get; set; }

        /// <summary>
        ///     How many steps are left before reaching last item.
        /// </summary>
        public int StepsLeft => Count - Cursor - 1;

        /// <summary>
        ///     Is the cursor pointing to the last item of the collection?
        /// </summary>
        public bool IsCursorAtEnd => Cursor == Count - 1;

        /// <summary>
        ///     Is the cursor pointing to the first item of the collection?
        /// </summary>
        public bool IsCursorAtStart => Cursor == 0;

        /// <summary>
        ///     How many steps already walked.
        /// </summary>
        public int StepsWalked => Cursor;

        /// <summary>
        ///     Gets current item, Walking[Cursor]. 
        /// </summary>
        public T Current => Walking[Cursor];

        /// <summary>
        ///     Can the walker continue next?
        /// </summary>
        public bool HasNext => Cursor + 1 < Count;

        /// <summary>
        ///     Can the walker continue back?
        /// </summary>
        public bool HasBack => Cursor - 1 >= 0;

        /// <summary>
        ///     Peaks back item. (Cursor-1), default if next has no item.
        /// </summary>
        public T PeakBack {
            get {
                var indx = Cursor - 1;
                if (indx < 0)
                    return default;
                return Walking[indx];
            }
        }

        /// <summary>
        ///     Peaks next item. (Cursor+1), default if next has no item.
        /// </summary>
        public T PeakNext {
            get {
                var indx = Cursor + 1;
                if (indx > Count)
                    return default;
                return Walking[indx];
            }
        }

        public ListWalker(IList<T> list) {
            Walking = list;
        }

        protected ListWalker() { }

        /// <summary>
        ///     The total count 
        /// </summary>
        public int Count => Walking.Count;

        /// <summary>
        ///     Sets <paramref name="obj"/> to current <see cref="Cursor"/>.
        /// </summary>
        public void SetCurrent(T obj) {
            Walking[Cursor] = obj;
        }

        /// <summary>
        ///     Sets cursor at <paramref name="index"/> position, note: 0 based!
        /// </summary>
        /// <param name="index"></param>
        public void Goto(int index) {
            Cursor = index;
            InvalidateCursor();
        }

        #region Back

        /// <summary>
        ///     Skips items forwards while <paramref name="predict"/> returns true.
        ///     <br></br>Note: Moves the cursor.
        /// </summary>
        /// <param name="predict"></param>
        /// <returns>The amount of items skipped.</returns>
        public int SkipBackwardWhile(Predicate<T> predict) {
            int c = 0;

            do {
                if (!predict(Current))
                    break;
                c++;
            } while (Back());

            return c;
        }

        /// <summary>
        ///     Skips items forwards while <paramref name="predict"/> returns true.
        ///     <br></br>Note: Moves the cursor.
        /// </summary>
        /// <param name="predict"></param>
        /// <returns>The amount of items skipped.</returns>
        public IEnumerable<T> BackwardWhile(Predicate<T> predict) {
            do {
                if (!predict(Current))
                    break;
                yield return Current;
            } while (Back());
        }


        /// <summary>
        ///     Skips items forwards while <paramref name="predict"/> returns true.
        ///     <br></br>Note: Moves the cursor.
        /// </summary>
        /// <param name="predict"></param>
        /// <returns>The amount of items skipped.</returns>
        public IEnumerable<T> TakeBackwardWhile(Predicate<T> predict, bool includingCurrent = false) {
            using (CheckPoint()) {
                if (!includingCurrent)
                    Back();
                do {
                    if (!predict(Current))
                        break;
                    yield return Current;
                } while (Back());
            }
        }

        /// <summary>
        ///     Will take all items from current cursor till collection enters.
        ///     <br></br>Note: Moves the cursor.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> AllBacks() {
            return BackwardWhile(_ => true);
        }

        /// <summary>
        ///     Will take all items from current cursor till collection enters.
        ///     <br></br>Note: Moves the cursor.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> TakeAllBacks(bool includingCurrent = false) {
            return TakeBackwardWhile(_ => true, includingCurrent);
        }

        /// <summary>
        ///     Moves to backward item or jumps <paramref name="steps"/> backward.
        ///     <br></br>Note: Moves the cursor.
        /// </summary>
        /// <param name="steps">How many steps to jump backwards.</param>
        /// <returns>True if there are more items, false if hit the end.</returns>
        public bool Back(int steps = 1) {
            Cursor -= steps;
            var ret = Cursor < 0;
            InvalidateCursor();
            return !ret;
        }

        /// <summary>
        ///     Takes backwards. see example.
        ///     <br></br>Note: DOESN'T Move the cursor.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="includingCurrent"></param>
        /// <returns></returns>
        /// <example>
        ///0,1,2,3,4,[5],6,7,8,9<br></br>
        ///includingCurrent = 1, take= 3 -> [5,4,3]<br></br>
        ///includingCurrent = 0, take= 3 -> [4,3,2]<br></br>
        ///includingCurrent = 0, take= 8 -> [4,3,2,1,0]<br></br>
        /// </example>
        public IEnumerable<T> TakeBack(int count, bool includingCurrent) {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            var to = InvalidateIndex(includingCurrent ? Cursor : Cursor - 1);
            var from = InvalidateIndex(Cursor - count);
            return Walking.Skip(from).Take(to - from + 1).Reverse();
        }

        /// <summary>
        ///     Skip and then take, backwards. see example.
        ///     <br></br>Note: DOESN'T Move the cursor.
        /// </summary>
        /// <param name="skip">How many to skip, 1 will do Cursor--;</param>
        /// <param name="take">How many items to take.</param>
        /// <returns></returns>
        /// <example>
        ///0,1,2,3,4,[5],6,7,8,9<br></br>
        ///skip = 1, take= 3 -> [4,3,2]<br></br>
        ///skip = 88, take= 3 -> []<br></br>
        ///skip = 1, take= 55 -> [4,3,2,1,0]<br></br>
        /// </example>
        public IEnumerable<T> SkipTakeBack(int skip, int take) {
            if (take < 0)
                throw new ArgumentOutOfRangeException(nameof(take));
            if (skip < 0)
                throw new ArgumentOutOfRangeException(nameof(skip));
            //count=6
            //skip = 1
            //from = 8
            //to = 9
            if (Cursor - skip < 0)
                return Enumerable.Empty<T>();
            return IterateReverse(Cursor - skip - take + 1, Cursor - skip);
        }

        #endregion

        #region Next

        /// <summary>
        ///     Skips items forwards while <paramref name="predict"/> returns true.
        ///     <br></br>Note: Moves the cursor.
        /// </summary>
        /// <param name="predict"></param>
        /// <returns>The amount of items skipped.</returns>
        public int SkipForwardWhile(Predicate<T> predict) {
            int c = 0;

            do {
                if (!predict(Current)) {
                    break;
                }
                c++;
            } while (Next());

            return c;
        }

        /// <summary>
        ///     Takes items while <paramref name="predict"/> returns true.
        ///     <br></br>Note: Moves the cursor.
        /// </summary>
        /// <param name="predict"></param>
        /// <returns>The amount of items skipped.</returns>
        public IEnumerable<T> TakeForwardWhile(Predicate<T> predict) {
            do {
                if (!predict(Current))
                    break;
                yield return Current;
            } while (Next());
        }

        /// <summary>
        ///     Will take all items from current cursor till collection ends.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> TakeAllNexts() {
            return TakeForwardWhile(_ => true);
        }

        /// <summary>
        ///     Moves to next item or jumps <paramref name="steps"/> forward.
        ///     <br></br>Note: Moves the cursor.
        /// </summary>
        /// <param name="steps">How many steps to jump forward.</param>
        /// <returns>True if there are more items, false if hit the end.</returns>
        public bool Next(int steps = 1) {
            Cursor += steps;
            bool ret = Cursor >= Count;
            InvalidateCursor();
            return !ret;
        }

        /// <summary>
        ///     Takes forwards. see example.
        ///     <br></br>Note: DOESN'T Move the cursor.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="includingCurrent"></param>
        /// <returns></returns>
        /// <example>
        ///0,1,2,3,4,[5],6,7,8,9<br></br>
        ///includingCurrent = 1, take= 3 -> [5,6,7]<br></br>
        ///includingCurrent = 0, take= 3 -> [6,7,8]<br></br>
        ///includingCurrent = 0, take= 8 -> [6,7,8,9]<br></br>
        /// </example>
        public IEnumerable<T> TakeNext(int count, bool includingCurrent) {
            //count=5
            //from = 7
            //to = 9
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            var to = InvalidateIndex(Cursor + count);
            var from = InvalidateIndex(includingCurrent ? Cursor : Cursor + 1);
            return Walking.Skip(from).Take(to - from + 1);
        }

        /// <summary>
        ///     Skip and then take, towards forward. see example.
        ///     <br></br>Note: DOESN'T Move the cursor.
        /// </summary>
        /// <param name="skip">How many to skip, 1 will do Cursor++;</param>
        /// <param name="take">How many items to take.</param>
        /// <returns></returns>
        /// <example>
        ///0,1,2,3,4,[5],6,7,8,9<br></br>
        ///skip = 1, take= 3 -> [6,7,8]<br></br>
        ///skip = 50, take= 3 -> []<br></br>
        ///skip = 1, take= 55 -> [6,7,8,9]<br></br>
        /// </example>
        public IEnumerable<T> SkipTakeNext(int skip, int take) {
            if (take < 0)
                throw new ArgumentOutOfRangeException(nameof(take));
            if (skip < 0)
                throw new ArgumentOutOfRangeException(nameof(skip));
            //count=6
            //skip = 1
            //from = 8
            //to = 9
            if (Cursor + skip >= Count)
                return Enumerable.Empty<T>();
            return Iterate(skip + Cursor, Cursor + skip + take - 1);
        }

        private IEnumerable<T> Iterate(int from, int to) {
            to = Math.Min(Count, to + 1);
            from = Math.Max(0, from);
            if (from >= to)
                yield break;
            for (int i = from; i < to; i++) {
                yield return Walking[i];
            }
        }

        private IEnumerable<T> IterateReverse(int from, int to) {
            to = Math.Min(Count, to + 1);
            from = Math.Max(0, from);
            if (from >= to)
                yield break;
            for (int i = to - 1; i >= @from; i--) {
                yield return Walking[i];
            }
        }

        #endregion

        #region Checkpoints

        /// <summary>
        ///     See example for how to use. Creates a checkpoint on creation and returns to it after dispose.
        /// </summary>
        /// <returns></returns>
        public IDisposable CheckPoint() {
            return new _CheckpointChanger(this, null);
        }

        /// <summary>
        ///     See example for how to use. Creates a checkpoint on creation and returns to it after dispose.
        /// </summary>
        /// <returns></returns>
        public IDisposable NamedCheckPoint(string name) {
            return new _CheckpointChanger(this, name);
        }

        /// <summary>
        ///     Saves a checkpoint with cursor's position for later use.
        /// </summary>
        /// <param name="name">The name for the checkpoint</param>
        public void SaveCheckpoint(string name) {
            Checkpoints[name] = Cursor;
        }

        /// <summary>
        ///     Saves a checkpoint (<paramref name="saveCurrentAs"/>) with cursor's position and enters an other checkpoint (<paramref name="enterTo"/>).
        /// </summary>
        /// <param name="saveCurrentAs">The name for the checkpoint that will be saved.</param>
        /// <param name="enterTo">The name of the checkpoint that will be entered after saving.</param>
        public void SaveAndEnterCheckpoint(string saveCurrentAs, string enterTo) {
            if (Checkpoints.ContainsKey(enterTo) == false)
                throw new ArgumentException(nameof(enterTo));

            SaveCheckpoint(saveCurrentAs);
            EnterCheckpoint(enterTo);
        }

        /// <summary>
        ///     Saves a checkpoint with cursor's position for later use.
        /// </summary>
        /// <param name="name"></param>
        public void EnterCheckpoint(string name) {
            if (Checkpoints.ContainsKey(name) == false)
                throw new ArgumentException(nameof(name));
            Cursor = Checkpoints[name];
        }

        /// <summary>
        ///     Removes given checkpoint.
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public bool RemoveCheckpoint(string cp) => Checkpoints.Remove(cp);

        /// <summary>
        ///     Clears all checkpoints.
        /// </summary>
        public void ClearCheckpoints() {
            //remove all points except of the disposables.
            foreach (var c in Checkpoints) {
                if (c.Key.StartsWith("___CheckpointChanger_"))
                    continue;
                Checkpoints.Remove(c.Key);
            }
        }

        private class _CheckpointChanger : IDisposable {
            private readonly ListWalker<T> _walker;
            private readonly string _checkpointName;

            /// <inheritdoc />
            public _CheckpointChanger(ListWalker<T> walker, string name) {
                _walker = walker ?? throw new ArgumentNullException(nameof(walker));
                _checkpointName = name ?? $"___CheckpointChanger_{Guid.NewGuid()}";
                _walker.SaveCheckpoint(_checkpointName);
            }

            /// <inheritdoc />
            public void Dispose() {
                _walker.EnterCheckpoint(_checkpointName);
                _walker.RemoveCheckpoint(_checkpointName);
            }
        }

        #endregion

        private void InvalidateCursor() {
            Cursor = InvalidateIndex(Cursor);
        }

        private int InvalidateIndex(int index) {
            if (index >= Count)
                index = Count - 1;
            else if (index < 0)
                index = 0;
            return index;
        }


        public void Reset() {
            Cursor = 0;
            ClearCheckpoints();
        }


        /// <inheritdoc />
        object ICloneable.Clone() {
            return Clone();
        }

        /// <summary>
        ///     Creates a copy, including all checkpoints.
        /// </summary>
        /// <returns></returns>
        public ListWalker<T> Clone() {
            return Clone(Walking);
        }

        /// <summary>
        ///     Creates a copy, including all checkpoints.
        /// </summary>
        /// <returns></returns>
        public ListWalker<TOut> Clone<TOut>(IList<TOut> toWalk) {
            var lw = new ListWalker<TOut>(toWalk);
            lw.Goto(Cursor);
            foreach (var c in Checkpoints) {
                if (c.Key.StartsWith("___CheckpointChanger_"))
                    continue;
                lw.Checkpoints.Add(c.Key, c.Value);
            }

            return lw;
        }

        #region Comparers

        private sealed class CursorRelationalComparer : IComparer<ListWalker<T>> {
            public int Compare(ListWalker<T> x, ListWalker<T> y) {
                if (ReferenceEquals(x, y))
                    return 0;
                if (ReferenceEquals(null, y))
                    return 1;
                if (ReferenceEquals(null, x))
                    return -1;
                return x.Cursor.CompareTo(y.Cursor);
            }
        }

        /// <inheritdoc />
        public int CompareTo(ListWalker<T> other) {
            if (ReferenceEquals(this, other))
                return 0;
            if (ReferenceEquals(null, other))
                return 1;
            return Cursor.CompareTo(other.Cursor);
        }

        /// <inheritdoc />
        public int CompareTo(object obj) {
            if (ReferenceEquals(null, obj))
                return 1;
            if (ReferenceEquals(this, obj))
                return 0;
            return obj is ListWalker<T> other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(ListWalker<T>)}");
        }

        public static bool operator <(ListWalker<T> left, ListWalker<T> right) {
            return Comparer<ListWalker<T>>.Default.Compare(left, right) < 0;
        }

        public static bool operator >(ListWalker<T> left, ListWalker<T> right) {
            return Comparer<ListWalker<T>>.Default.Compare(left, right) > 0;
        }

        public static bool operator <=(ListWalker<T> left, ListWalker<T> right) {
            return Comparer<ListWalker<T>>.Default.Compare(left, right) <= 0;
        }

        public static bool operator >=(ListWalker<T> left, ListWalker<T> right) {
            return Comparer<ListWalker<T>>.Default.Compare(left, right) >= 0;
        }

        public static IComparer<ListWalker<T>> CursorComparer { get; } = new CursorRelationalComparer();

        private sealed class WalkingCursorEqualityComparer : IEqualityComparer<ListWalker<T>> {
            public bool Equals(ListWalker<T> x, ListWalker<T> y) {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return Equals(x.Walking, y.Walking) && x.Cursor == y.Cursor;
            }

            public int GetHashCode(ListWalker<T> obj) {
                unchecked {
                    return ((obj.Walking != null ? obj.Walking.GetHashCode() : 0) * 397) ^ obj.Cursor;
                }
            }
        }

        public static IEqualityComparer<ListWalker<T>> WalkingCursorComparer { get; } = new WalkingCursorEqualityComparer();

        /// <inheritdoc />
        public bool Equals(ListWalker<T> other) {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(Walking, other.Walking) && Cursor == other.Cursor;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((ListWalker<T>) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            unchecked {
                return ((Walking != null ? Walking.GetHashCode() : 0) * 397) ^ Cursor;
            }
        }

        public static bool operator ==(ListWalker<T> left, ListWalker<T> right) {
            return Equals(left, right);
        }

        public static bool operator !=(ListWalker<T> left, ListWalker<T> right) {
            return !Equals(left, right);
        }

        #endregion
    }
}