using System;
using System.Collections.Generic;
using System.Linq;
using Regen.Compiler;
using Regen.Compiler.Helpers;
using Regen.Exceptions;
using Regen.Helpers;
using Regen.Helpers.Collections;
using Regen.Parser.Expressions;

namespace Regen.Parser {
    public class ParserAction {
        public ParserToken Token { get; set; }

        public List<Expression> Related { get; set; }

        public List<Line> RelatedLines { get; set; }

        public bool Consumed { get; set; }

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

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() {
            return $"{nameof(Token)}: {Token}, {nameof(Related)}: {Related.FirstOrDefault()}";
        }
    }

    /// <summary>
    ///     Parses text file that has been lexxed by <see cref="ExpressionLexer"/> into <see cref="ParserAction"/>s that ressemble a combination of tokens (<see cref="ParserToken"/>)
    /// </summary>
    public static class ExpressionParser {
        public static ParsedCode Parse(string code, Dictionary<string, object> variables = null, InterpreterOptions opts = null) {
            code = code.Replace("\r", ""); //todo this might cause throws in osx.
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
                            case ExpressionToken.Template: {
                                //this is import %import namespace.type as aliasnmae
                                var template = TemplateExpression.Parse(ew);
                                parserTokens += new ParserAction(ParserToken.Template, output.MarkDeleteLinesRelated(template.Matches()), template);
                                break;
                            }

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
                                    var expr = VariableDeclarationExpression.Parse(ew);
                                    parserTokens += new ParserAction(ParserToken.Declaration, output.MarkDeleteLinesRelated(expr.Matches()), expr);
                                } else {
                                    break;
                                }

                                break;
                            }

                            case ExpressionToken.LeftParen: {
                                //it is an expression block %(expr)
                                ew.NextOrThrow();

                                var expr = Expression.ParseExpression(ew, typeof(ExpressionParser));
                                parserTokens += new ParserAction(ParserToken.Expression, output.MarkDeleteLinesRelated(expr.Matches()), expr);
                                ew.IsCurrentOrThrow(ExpressionToken.RightParen);
                                ew.Next();
                                break;
                            }

                            case ExpressionToken.Foreach: {
                                parserTokens += ForeachExpression.Parse(ew, code, output);
                                break;
                            }

                            case ExpressionToken.CommentRow:
                                //skip untill we hit newline
                                ew.SkipForwardWhile(t => t.Token != ExpressionToken.NewLine); //todo test
                                break;

                            default: {
                                var precentageLine = output.GetLineAt(ew.PeakBack.Match.Index);
                                if (precentageLine.CleanContent() != "%")
                                    throw new UnexpectedTokenException(current.Token, $"The given token was not expected at line {precentageLine.LineNumber}, offset: {current.Match.Index - precentageLine.StartIndex}");
                                break;
                            }
                        }

                        break;
                    }

                    default:
                        break;
                }
            } while (ew.Next());

            return new ParsedCode() {
                OriginalCode = code,
                Output = output,
                Variables = variables,
                ETokens = (List<TokenMatch>) ew.Walking,
                ParseActions = parserTokens,
                Options = opts
            };
        }
    }
}