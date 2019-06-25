using System;
using System.Collections.Generic;

namespace Regen.Compiler.Helpers {
    public class StringExpander {
        public string Content {
            get => string.Join("", _splits);
            set {
                if (value == null)
                    throw new NullReferenceException();
                _splits.Clear();
                _splits.Add(value);
            }
        }

        private readonly List<string> _splits = new List<string>();

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public StringExpander(string content) {
            Content = content;
        }

        public void Replace(int index, string replacement) {
            var accum = 0;
            var cnt = _splits.Count;
            for (int i = 0; i < cnt; i++) {
                var currSplit = _splits[i];
                if (index >= accum && index <= accum + _splits[i].Length - 1) {
                    _splits.RemoveAt(i);
                    _splits.Insert(i, replacement);
                    break;
                }

                accum += currSplit.Length;
            }
        }

        public void SplitAt(int index, bool swallowIndex, bool goesleft = true) {
            var accum = 0;
            var cnt = _splits.Count;
            for (int i = 0; i < cnt; i++) {
                var currSplit = _splits[i];
                if (index >= accum && index <= accum + _splits[i].Length - 1) {
                    var localindex = index - accum;
                    var left = currSplit.Substring(0, localindex + (swallowIndex ? (goesleft ? 0 : 1) : 1));
                    var right = currSplit.Substring(localindex + (goesleft ? 1 : 0));
                    _splits.RemoveAt(i);
                    _splits.Insert(i, right);
                    _splits.Insert(i, left);
                    break;
                }

                accum += currSplit.Length;
            }
        }
    }
}