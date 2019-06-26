using System;
using System.Collections.Generic;
using System.Linq;

namespace Regen.Compiler.Helpers {
    public class Line : ICloneable, IEquatable<Line> {
        public List<string> Metadata { get; protected set; } = new List<string>();

        public Line(StringSpan content, int lineNumber, int startIndex, int endIndex) {
            _content = content;
            LineNumber = lineNumber;
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        public Line(StringSpan content, int lineNumber) {
            _content = content;
            LineNumber = lineNumber;
        }

        private readonly StringSpan _content;
        public Guid Id { get; private set; } = Guid.NewGuid();

        public int LineNumber { get; set; }

        public string Content => _content.ToString();

        public bool ContentWasModified { get; set; }

        public int StartIndex { get; set; }

        /// <summary>
        ///     The original end index, doesn't change after modification - to get the newest index, use <see cref="ComputeEndIndex"/>
        /// </summary>
        public int EndIndex { get; set; }

        /// <summary>
        ///     Compute the end index of this line. start index + content length.
        /// </summary>
        public int ComputeEndIndex => StartIndex + _content.Length;

        /// <summary>
        ///     If this is true, this line will be removed during <see cref="LineBuilder.Combine"/>
        /// </summary>
        public bool MarkedForDeletion { get; set; }

        public bool IsEmpty => (_content as StringSlice)?.Deleted ?? false || _content.Length == 0;

        public bool IsJustSpaces => string.IsNullOrEmpty(CleanContent(false));


        /// <summary>
        ///     Sums all whitespace and tabs before the content starts. Used for adding extra lines here.
        /// </summary>
        public string Prepends => new string(_content.Chars.TakeWhile(c => c == ' ' || c == '\t').ToArray());

        public int Length => _content.Length;

        /// <summary>
        ///     Returns a content string from trailing spaces, newlines and tabs.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Does not modify the content itself</remarks>
        public string CleanContent(bool save = false) {
            if (save) {
                _content.Trim(' ', '\t', '\n', '\r');
                return _content.ToString();
            }

            return this._content.ToString().Trim(' ', '\t', '\n', '\r');
        }

        /// <summary>
        ///     Replaces current Content if it wasn't modified, otherwise appends.
        /// </summary>
        /// <param name="line"></param>
        /// <remarks>Make sure that every <see cref="line"/> that is added has \n\r at its end.</remarks>
        public void ReplaceOrAppend(string line) {
            if (ContentWasModified) {
                _content.Add(line); //we use Content to also set ContentWasModified
            } else {
                _content.ReplaceWith(line); //we use Content to also set ContentWasModified
                ContentWasModified = true;
            }
        }

        /// <summary>
        ///     Replaces current Content.
        /// </summary>
        /// <param name="line">The new line's content.</param>
        public void Replace(string line) {
            _content.ReplaceWith(line); //we use Content to also set ContentWasModified
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() {
            return $"{StartIndex}-{EndIndex} | {Content}";
        }

        /// <summary>Creates a new object that is a copy of the current instance.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone() {
            return new Line((StringSpan) _content.Clone(), LineNumber, StartIndex, EndIndex) {Id = Id, Metadata = Metadata, MarkedForDeletion = MarkedForDeletion, ContentWasModified = ContentWasModified};
        }

        #region Equality

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(Line other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Line) obj);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Id.GetHashCode();
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Regen.Parser.Interperter.LineBuilder.Line" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(Line left, Line right) {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Regen.Parser.Interperter.LineBuilder.Line" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(Line left, Line right) {
            return !Equals(left, right);
        }

        #endregion
    }
}