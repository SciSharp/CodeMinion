using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Regen.Compiler.Helpers;
using Regen.Exceptions;
using Regen.Helpers;

namespace Regen.Parser.Expressions {
    public class ForeachExpression : Expression {
        public ArgumentsExpression Arguments;
        public string Content;
        public int Depth = 0;

        public static ParserAction Parse(string code) {
            var walker = new ExpressionWalker(ExpressionLexer.Tokenize(code));
            if (walker.Current.Token == ExpressionToken.Mod)
                walker.NextOrThrow();
            var lines = new LineBuilder(code);
            lines.Lines.RemoveAt(0);
            return Parse(walker, code, lines);
        }

        public static ParserAction Parse(ExpressionWalker ew, string code, LineBuilder output) {
            //  multiline:
            //      %foreach expr%
            //        code #1
            //      %
            //  singleline:
            //      %foreach expr
            //          code #1
            ew.IsCurrentOrThrow(ExpressionToken.Foreach);
            ew.NextOrThrow();
            //parse the arguments for the foreach
            var args = ArgumentsExpression.Parse(ew, token => token.Token == ExpressionToken.NewLine || token.Token == ExpressionToken.Mod, false, typeof(ForeachExpression));
            //ew.Back(); //agrumentsExpression skips the closer token and we need it to identify if this is a singleline or multiline
            if (output == null)
                output = new LineBuilder(code);

            string content;
            var relatedLines = new List<Line>();
            relatedLines.AddRange(output.GetLinesRelated(args.Matches()));
            if (ew.PeakBack.Token == ExpressionToken.Mod) {
                //the content is % to % block
                var leftBorder = ew.Current.Match;
                var nextMod = ForeachExpression.FindCloser(leftBorder.Index, code);
                //handle implicit end block (when % is not existing)
                if (nextMod == -1) nextMod = code.Length - 1;
                ew.SkipForwardWhile(token => token.Match.Index < nextMod);
                ew.Next(); //skip % itself
                var l1 = output.GetLineAt(leftBorder.Index) ?? output.Lines.First();

                relatedLines.AddRange(new Range(l1.LineNumber, output.GetLineAt(nextMod).LineNumber).EnumerateIndexes().Select(output.GetLineByLineNumber));
                content = null;
            } else {
                //the content is only next line
                var leftMod = ew.Current.Match;
                var nextMod = code.IndexOf('\n', leftMod.Index);
                relatedLines.Add(output.GetLineByLineNumber(relatedLines.Last().LineNumber + 1)); //next line.
                content = code.Substring(leftMod.Index, nextMod == -1 ? (code.Length - leftMod.Index) : nextMod - leftMod.Index);
            }

            relatedLines = relatedLines.Distinct().OrderBy(l => l.StartIndex).ToList();

            if (relatedLines.Count(l=>l.Content.Contains("%foreach")) <= relatedLines.Count(l => l.CleanContent() == "%") && relatedLines.Last().CleanContent() == "%") {
                relatedLines[relatedLines.Count - 1].MarkedForDeletion = true;
                relatedLines.RemoveAt(relatedLines.Count - 1);
            }
           
            //make sure to clean out % at the end
            if (content == null)
                content = relatedLines.Select(l => l.Content).StringJoin();


            //all lines of the foreach are destined to deletion
            foreach (var line in relatedLines)
                line.MarkedForDeletion = true;

            return new ParserAction(ParserToken.ForeachLoop, relatedLines, new ForeachExpression() {Content = content, Arguments = args});
        }

        /// <summary>
        ///     Finds the index where current foreach closes.
        /// </summary>
        /// <param name="offset">The index of opening %</param>
        /// <returns>index at the last closer (%)</returns>
        /// <remarks>Supports nested foreach</remarks>
        public static int FindCloser(int offset, string str) {
            const string denier = "foreach";
            int ret = 0;
            int at = offset;
            int parens = 1;
            do {
                _retry:
                var found = str.IndexOf('%', at) + 1;
                if (str[Math.Max(found - 2, 0)] == '\\') {
                    at = found;
                    goto _retry;
                }

                if (found == 0) //note the + 1 above
                    throw new UnexpectedTokenException("Was unable to find % for a foreach loop. Nested foreach loops does not support single-line foreach loops.");
                if (found + denier.Length <= str.Length && str.Substring(found, denier.Length) == denier)
                    parens++;
                else
                    parens--;

                ret = found - 1;
                at = str.IndexOf('\n', found);
                if (at == -1)
                    at = found;
            } while (parens > 0);

            return ret;
        }

        public override IEnumerable<Expression> Iterate() {
            return this.Yield<Expression>().Concat(Arguments.Iterate());
        }
    }
}