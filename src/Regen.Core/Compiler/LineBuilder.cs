using System;
using System.Collections.Generic;
using System.Linq;

namespace Regen.Compiler {
    public class LineBuilder : ICloneable {
        public List<Line> Lines { get; set; }

        protected LineBuilder() { }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public LineBuilder(string txt) {
            Lines = txt
                .Split('\n')
                .Select((str, i) => new Line(str + "\n", i + 1))
                .ToList();

            //generate indexes
            Line curr = Lines[0];
            curr.StartIndex = 0;
            curr.EndIndex = curr.Content.Length - 1;
            curr.EndIndex = curr.IsEmpty ? 0 : curr.Content.Length - 1;
            Line prev;

            for (int i = 1; i < Lines.Count; i++) {
                prev = Lines[i - 1];
                curr = Lines[i];
                curr.StartIndex = prev.EndIndex + 1;
                curr.EndIndex = curr.StartIndex + (curr.IsEmpty ? 0 : curr.Content.Length - 1);
            }
        }


        public Line GetLineAt(int index) {
            return Lines.Single(l => l.StartIndex <= index && l.EndIndex >= index);
        }

        /// <summary>
        ///     Combines all lines into a single string.
        /// </summary>
        public string Compile() {
            var validLines = Lines.Where(line => !line.MarkedForDeletion).ToArray();

            //clean trailing lines at the beggining and end
            foreach (var line in validLines.TakeWhile(l => l.IsJustSpaces)) {
                line.MarkedForDeletion = true;
            }

            foreach (var line in validLines.Reverse().TakeWhile(l => l.IsJustSpaces)) {
                line.MarkedForDeletion = true;
            }

            validLines = Lines.Where(line => !line.MarkedForDeletion).ToArray();

            var compiled = string.Join("", validLines.Select(l => l.Content));
            return compiled.Trim('\n', '\r') + Environment.NewLine;
        }

        /// <summary>Creates a new object that is a copy of the current instance.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        object ICloneable.Clone() {
            return Clone();
        }

        /// <summary>Creates a new object that is a copy of the current instance.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public LineBuilder Clone() {
            var builder = new LineBuilder();
            builder.Lines = Lines.Select(l => (Line) l.Clone()).ToList();
            return builder;
        }

        public Line FindLine(Line line) {
            return Lines.SingleOrDefault(l => l == line);
        }
    }


    public class Line : ICloneable, IEquatable<Line> {
        public Line(string content, int lineNumber, int startIndex, int endIndex) {
            _content = content;
            LineNumber = lineNumber;
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        public Line(string content, int lineNumber) {
            _content = content;
            LineNumber = lineNumber;
        }

        private string _content;

        public Guid Id { get; private set; } = Guid.NewGuid();

        public int LineNumber { get; set; }

        public string Content {
            get => _content;
            set {
                _content = value;
                ContentWasModified = true;
            }
        }

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
        ///     If this is true, this line will be removed during <see cref="LineBuilder.Compile"/>
        /// </summary>
        public bool MarkedForDeletion { get; set; }

        public bool IsEmpty => string.IsNullOrEmpty(this.Content);

        public bool IsJustSpaces => string.IsNullOrEmpty(CleanContent());


        /// <summary>
        ///     Sums all whitespace and tabs before the content starts. Used for adding extra lines here.
        /// </summary>
        public string Prepends => new string(_content.TakeWhile(c => c == ' ' || c == '\t').ToArray());

        /// <summary>
        ///     Returns a content string from trailing spaces, newlines and tabs.
        /// </summary>
        /// <returns></returns>
        public string CleanContent() {
            return this._content.Trim(' ', '\t', '\n', '\r');
        }

        /// <summary>
        ///     Replaces current Content if it wasn't modified, otherwise appends.
        /// </summary>
        /// <param name="line"></param>
        /// <remarks>Make sure that every <see cref="line"/> that is added has \n\r at its end.</remarks>
        public void ReplaceOrAppend(string line) {
            if (ContentWasModified) {
                Content += line; //we use Content to also set ContentWasModified
            } else
                Content = line; //we use Content to also set ContentWasModified
        }
        /// <summary>
        ///     Replaces current Content.
        /// </summary>
        /// <param name="line">The new line's content.</param>
        public void Replace(string line) {
            Content = line; //we use Content to also set ContentWasModified
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() {
            return $"{StartIndex}-{EndIndex} | {Content}";
        }

        /// <summary>Creates a new object that is a copy of the current instance.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone() {
            return new Line(_content, LineNumber, StartIndex, EndIndex) {Id = Id};
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