using System;

namespace Regen.Helpers.Collections {
    /// <summary>
    ///     NonGeneric - Allows traveling an array, take, skipping, checkpoint system and more.
    /// </summary>
    /// <typeparam name="T">The element type of the array.</typeparam>
    public interface IListWalker : ICloneable {
        /// <summary>
        ///     The index the walker is currently at.
        /// </summary>
        int Cursor { get; set; }

        /// <summary>
        ///     How many steps are left before reaching last item.
        /// </summary>
        int StepsLeft { get; }

        /// <summary>
        ///     Is the cursor pointing to the last item of the collection?
        /// </summary>
        bool IsCursorAtEnd { get; }

        /// <summary>
        ///     Is the cursor pointing to the first item of the collection?
        /// </summary>
        bool IsCursorAtStart { get; }

        /// <summary>
        ///     How many steps already walked.
        /// </summary>
        int StepsWalked { get; }

        /// <summary>
        ///     The total count 
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     Sets cursor at <paramref name="index"/> position, note: 0 based!
        /// </summary>
        /// <param name="index"></param>
        void Goto(int index);

        /// <summary>
        ///     Moves to backward item or jumps <paramref name="steps"/> backward.
        ///     <br></br>Note: Moves the cursor.
        /// </summary>
        /// <param name="steps">How many steps to jump backwards.</param>
        /// <returns>True if there are more items, false if hit the end.</returns>
        bool Back(int steps = 1);

        /// <summary>
        ///     Moves to next item or jumps <paramref name="steps"/> forward.
        ///     <br></br>Note: Moves the cursor.
        /// </summary>
        /// <param name="steps">How many steps to jump forward.</param>
        /// <returns>True if there are more items, false if hit the end.</returns>
        bool Next(int steps = 1);

        /// <summary>
        ///     See example for how to use. Creates a checkpoint on creation and returns to it after dispose.
        /// </summary>
        /// <returns></returns>
        IDisposable CheckPoint();

        /// <summary>
        ///     See example for how to use. Creates a checkpoint on creation and returns to it after dispose.
        /// </summary>
        /// <returns></returns>
        IDisposable NamedCheckPoint(string name);

        /// <summary>
        ///     Saves a checkpoint with cursor's position for later use.
        /// </summary>
        /// <param name="name">The name for the checkpoint</param>
        void SaveCheckpoint(string name);

        /// <summary>
        ///     Saves a checkpoint (<paramref name="saveCurrentAs"/>) with cursor's position and enters an other checkpoint (<paramref name="enterTo"/>).
        /// </summary>
        /// <param name="saveCurrentAs">The name for the checkpoint that will be saved.</param>
        /// <param name="enterTo">The name of the checkpoint that will be entered after saving.</param>
        void SaveAndEnterCheckpoint(string saveCurrentAs, string enterTo);

        /// <summary>
        ///     Saves a checkpoint with cursor's position for later use.
        /// </summary>
        /// <param name="name"></param>
        void EnterCheckpoint(string name);

        /// <summary>
        ///     Removes given checkpoint.
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        bool RemoveCheckpoint(string cp);

        /// <summary>
        ///     Clears all checkpoints.
        /// </summary>
        void ClearCheckpoints();

        void Reset();
    }
}