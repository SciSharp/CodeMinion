using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Flee.PublicTypes;
using Regen.Builtins;
using Regen.Collections;
using Regen.Compiler.Digest;
using Regen.Compiler.Helpers;
using Regen.DataTypes;
using Regen.Exceptions;
using Regen.Helpers;
using Regen.Wrappers;
using ExpressionCompileException = Regen.Exceptions.ExpressionCompileException;

namespace Regen.Compiler.Expressions {
    public class ParserAction {
        public ParserToken Token { get; set; }

        public List<Expression> Related { get; set; }
        public List<Line> RelatedLines { get; set; }

        public ParserAction(ParserToken token, IList<Expression> related, params Line[] lines) {
            Token = token;
            RelatedLines = lines.ToList();
            Related = related.ToList();
        }

        public ParserAction(ParserToken token, IList<Line> lines, params Expression[] related) {
            RelatedLines = lines.ToList();
            Token = token;
            Related = related.ToList();
        }
    }

    /// <summary>
    ///     Parses text file that has been lexxed by <see cref="ExpressionLexer"/> into <see cref="ParserAction"/>s that ressemble a combination of tokens (<see cref="ParserToken"/>)
    /// </summary>
    public static class ExpressionParser {
        public static ParsedCode Interpret(string code, Dictionary<string, object> variables = null, InterpreterOptions opts = null) {
            code = code.Replace("\r", "");
            StringSpan output_sb;
            var output = new LineBuilder(output_sb = StringSpan.Create(code));
            variables = variables ?? new Dictionary<string, object>();
            opts = opts ?? new InterpreterOptions();
            // Define the context of our expression
            var ew = new ExpressionWalker(ExpressionLexer.Tokenize(code).Where(t => t.Token != ExpressionToken.UnixNewLine).ToList());

            //if no tokens detected
            if (ew.Count == 0) {
                return new ParsedCode() {OriginalCode = code, Output = output, Variables = variables, Options = opts, ParseActions = new OList<ParserAction>(0)};
            }

            var parserTokens = new OList<ParserAction>();
            do {
                var current = ew.Current;

                switch (ew.Current.Token) {
                    case ExpressionToken.Mod: {
                        //copypastes.Add(sb.Substring(from, ew.Current.Match.Index + ew.Current.Match.Length - 1));
                        current = ew.NextToken();
                        if (current == null)
                            break;
                        switch (current.Token) {
                            case ExpressionToken.Import: {
                                //this is import %import namespace.type as aliasnmae
                                var import = ImportExpression.Parse(ew);
                                parserTokens += new ParserAction(ParserToken.Import, output.MarkDeleteLinesRelated(import.Matches()), import);
                                break;
                            }

                            case ExpressionToken.Literal: {
                                //this is variable declaration %varname = expr
                                var peak = ew.PeakNext.Token;
                                if (peak == ExpressionToken.Equal) {
                                    var expr = ew.ParseVariable();
                                    parserTokens += new ParserAction(ParserToken.Declaration, output.MarkDeleteLinesRelated(expr.Matches()), expr);
                                } else {
                                    break;
                                }

                                break;
                            }

                            case ExpressionToken.LeftParen: {
                                //it is an expression block %(expr)
                                ew.NextOrThrow();

                                var expr = Expression.ParseExpression(ew);
                                parserTokens += new ParserAction(ParserToken.Expression, output.MarkDeleteLinesRelated(expr.Matches()), expr);
                                ew.IsCurrentOrThrow(ExpressionToken.RightParen);
                                ew.Next();
                                break;
                            }

                            case ExpressionToken.Foreach: {
                                //%foreach expr%
                                //  code $1
                                //%

                                //get 
                                ew.IsCurrentOrThrow(ExpressionToken.Foreach);
                                ew.NextOrThrow();
                                //parse the arguments for the foreach
                                var args = ArgumentsExpression.Parse(ew, token => token.Token == ExpressionToken.NewLine || token.Token == ExpressionToken.Mod, false, typeof(ForeachExpression));
                                if (ew.IsCurrent(ExpressionToken.NewLine)) {
                                    ew.NextOrThrow();
                                }

                                StringSlice content;
                                var relatedLines = new List<Line>();
                                relatedLines.AddRange(output.GetLinesRelated(args.Matches()));
                                if (ew.PeakBack.Token == ExpressionToken.Mod) {
                                    //the content is % to % block
                                    var leftMod = ew.Current.Match;
                                    var nextMod = code.IndexOf('%', leftMod.Index);
                                    content = output_sb.Substring(leftMod.Index, nextMod == -1 ? code.Length - leftMod.Index : nextMod - leftMod.Index);
                                    ew.SkipForwardWhile(token => token.Token != ExpressionToken.Mod);
                                    ew.Next(); //skip % itself

                                    relatedLines.AddRange(new Range(output.GetLineAt(leftMod.Index).LineNumber, output.GetLineAt(nextMod).LineNumber)
                                        .EnumerateIndexes().Select(i => output.GetLineByLineNumber(i)));
                                } else {
                                    //the content is only next line
                                    var leftMod = ew.Current.Match;
                                    var nextMod = code.IndexOf('\n', leftMod.Index);
                                    relatedLines.Add(output.GetLineByLineNumber(relatedLines.Last().LineNumber + 1)); //next line.
                                    content = output_sb.Substring(leftMod.Index, nextMod == -1 ? (code.Length - leftMod.Index) : nextMod - leftMod.Index);
                                }

                                //all lines of the foreach are destined to deletion
                                foreach (var line in relatedLines) {
                                    line.MarkedForDeletion = true;
                                }

                                Console.WriteLine(content.ToString());
                                ForeachExpression expr = new ForeachExpression() {Content = content, Arguments = args};
                                parserTokens += new ParserAction(ParserToken.ForeachLoop, relatedLines, expr);
                                break;
                            }

                            case ExpressionToken.CommentRow:
                                //skip untill we hit newline
                                ew.SkipForwardWhile(t => t.Token != ExpressionToken.NewLine); //todo test
                                break;

                            default: {
                                var precentageLine = output.GetLineAt(ew.PeakBack.Match.Index);
                                if (precentageLine.CleanContent() != "%")
                                    throw new InvalidTokenException(current.Token, $"The given token was not expected at line {precentageLine.LineNumber}, offset: {current.Match.Index - precentageLine.StartIndex}");
                                break;
                            }
                        }

                        //from = ew.Current.Match.Index + ew.Current.Match.Length + 1;
                        //to = from;
                        break;
                    }

                    default:
                        //to = ew.Current.Match.Index + ew.Current.Match.Length;
                        //Console.WriteLine($"Ignored {current.Token} ({current.Match.Value})");
                        break;
                }
            } while (ew.Next());

            return new ParsedCode() {
                OriginalCode = code,
                Output = output,
                Variables = variables,
                ETokens = (List<EToken>) ew.Walking,
                ParseActions = parserTokens,
                Options = opts
            };
        }
    }
}