using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Regen.Compiler.Expressions;
using Regen.Parser;

namespace Regen.Compiler.Helpers {
    public class LineBuilder : ICloneable {
        public List<Line> Lines { get; set; }

        protected LineBuilder() { }

        public LineBuilder(string txt) : this(StringSpan.Create(txt)) { }

        public LineBuilder(StringSpan txt) {
            Lines = txt
                .Split('\n', StringSplitOptions.None)
                .Select((span, i) => {
                    if (span.Start == -1 || span.End == -1) {
                        return new Line(new StringSource("\n"), i + 1);
                    }

                    if (span[span.Length - 1] != '\n')
                        span.Extend(1); //add swollen newline
                    return new Line(span, i + 1);
                })
                .ToList();

            //generate indexes
            Line curr = Lines[0];
            curr.StartIndex = 0;
            curr.EndIndex = curr.Length - 1;
            curr.EndIndex = curr.IsEmpty ? 0 : curr.Length - 1;
            Line prev;

            for (int i = 1; i < Lines.Count; i++) {
                prev = Lines[i - 1];
                curr = Lines[i];
                curr.StartIndex = prev.EndIndex + 1;
                curr.EndIndex = curr.StartIndex + (curr.IsEmpty ? 0 : curr.Length - 1);
            }
        }

        public Line GetLineAt(int index) {
            return Lines.FirstOrDefault(l => l.StartIndex <= index && l.EndIndex >= index);
        }

        public Line GetLineByLineNumber(int lineNumber) {
            return Lines.FirstOrDefault(l => l.LineNumber == lineNumber);
        }

        public Line[] GetLinesAt(IEnumerable<int> indexes) {
            return indexes.Select(GetLineAt).Where(v => v != null).Distinct().ToArray();
        }

        public Line[] GetLinesRelated(IEnumerable<RegexResult> matches) {
            var enumerable = matches.ToArray();
            return this.GetLinesAt(enumerable.Where(m => m.Index != -1).SelectMany(m => new int[] {m.Index, m.Index + m.Length - 1}).Distinct()).ToArray();
        }

        public Line[] MarkDeleteLinesRelated(IEnumerable<RegexResult> matches) {
            var lines = GetLinesRelated(matches);
            foreach (var affectedLine in lines) {
                affectedLine.MarkedForDeletion = true;
            }

            return lines;
        }

        /// <summary>
        ///     Combines all lines into a single string.
        /// </summary>
        public string Combine(InterpreterOptions opts = null) {
            var validLines = Lines.Where(line => !line.MarkedForDeletion).ToList();

            //clean trailing lines at the beggining and end
            foreach (var line in validLines.TakeWhile(l => l.IsJustSpaces)) {
                line.MarkedForDeletion = true;
            }

            foreach (var line in ((IList<Line>) validLines).Reverse().TakeWhile(l => l.IsJustSpaces)) {
                line.MarkedForDeletion = true;
            }

            //delete again
            validLines = Lines.Where(line => !line.MarkedForDeletion).ToList();

            //handle ClearLoneBlockmarkers
            if (opts != null && opts.ClearLoneBlockmarkers) {
                validLines.RemoveAll(l => l.Content.Trim('\n', '\r', '\t', ' ', '\0') == "%");
            }

            //handle UnespacePrecentages
            if (opts != null && opts.UnespacePrecentages) {
                foreach (var validLine in validLines) {
                    if (validLine.Content.Contains("\\%"))
                        validLine.Replace(validLine.Content.Replace("\\%", "%"));
                }
            }

            //todo this might fail, we might need to add \n at the end of each validLine
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
}