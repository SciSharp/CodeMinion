using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text.RegularExpressions;
using Regen.Compiler;
using Regen.Flee.PublicTypes;
using Regen.Helpers;
using Regen.Parser;

namespace Regen.DataTypes {
    public partial class Array {
        [DebuggerDisplay("{Depth} - {Match}")]
        private class ExpressionTrack {
            public Guid Id { get; } = Guid.NewGuid();
            private Match _match;
            public string AssignedVariable { get; set; }

            public Match Match {
                get => _match;
                set {
                    _match = value;
                    Index = value.Index;
                    Length = value.Length;
                }
            }

            public int Depth { get; set; }
            public int Index { get; set; }
            public int Length { get; set; }
        }

        public static Array Parse(string @string, VariableCollection variables, IEvaluator evaluator) {
            const string arrayElementsSeperationRegex = @"(?<!\\)\|";

            const string BracketDepthPattern2 = @"\[([^\[\]]*)\]";
            var arrayStr = @string;

            //here we perform an algorithm similar to shunting-yard 
            //var parsed = new Dictionary<Guid, Data>();
            //var parsedMap = new Dictionary<string, string>();
            var tracks = new List<ExpressionTrack>();
            string @in = arrayStr;
            int depth = 0;
            int i = 0;
            Array _last = null;
            var uniqueid = new string(Guid.NewGuid().ToString("N").SkipWhile(char.IsDigit).ToArray());
            while (true) {
                var matches = Regex.Matches(@in, BracketDepthPattern2, Regexes.DefaultRegexOptions);
                if (matches.Count == 0)
                    break;

                foreach (Match expressionMatch in matches.Cast<Match>().Reverse()) { //iterate matches, last to first
                    //expressionMatch.DisplayMatchResults();
                    var expression = StripBrackets(expressionMatch.Value);
                    var expressionTrack = new ExpressionTrack() {Depth = depth, Match = expressionMatch};
                    tracks.Add(expressionTrack);

                    //the following might be useless, left here in-case problems rise up.
                    //for (int j = 0; j < tracks.Count - 1; j++) { 
                    //    expression = expression.Replace(
                    //        tracks[j].Match.Value,
                    //        tracks[j].AssignedVariable);
                    //}

                    var arr = _last = ParseArray(expression); // parsed[expressionTrack.Id] =
                    var key = $"__{uniqueid}{i++}";
                    variables.Add(key, arr);
                    expressionTrack.AssignedVariable = key; //parsedMap[expressionTrack.Match.Value] = 
                    @in = @in.Replace(expressionMatch.Value, expressionTrack.AssignedVariable);
                }
            }

            //remove used variables
            foreach (var k in variables.Keys.ToArray().Where(k => k.StartsWith($"__{uniqueid}"))) {
                variables.Remove(k);
            }

            return _last;

            string StripBrackets(string input) {
                if (input.StartsWith("[") && input.EndsWith("]"))
                    input = input.Substring(1, input.Length - 2);
                return input;
            }

            Array ParseArray(string input) {
                input = StripBrackets(input);

                var _parsed = Regex.Split(input, arrayElementsSeperationRegex, Regexes.DefaultRegexOptions)
                    .Select(v => v.Replace("\\|", "|")) //unescape
                    .Select(v => v == string.Empty ? "\"\"" : v) //filter empty values to empty string
                    .Select(v => evaluator.EvaluateObject(v))
                    .Select(Data.Create)
                    .ToList();

                return new Array(_parsed);
            }
        }
    }
}