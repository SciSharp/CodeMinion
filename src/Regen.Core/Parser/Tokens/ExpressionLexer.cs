using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Regen.Exceptions;
using Regen.Helpers;
using Regen.Helpers.Collections;

namespace Regen.Parser {
    public class ExpressionLexer {
        public static List<TokenMatch> Tokenize(string codeStr, params ExpressionToken[] exceptFor) {
            var code = new StringBuilder(codeStr);
            var allTokens = new List<(string Regex, ExpressionToken Token, List<ExpressionToken> Swallows, int Order)>(
                Enum.GetValues(typeof(ExpressionToken)).Cast<ExpressionToken>()
                    .Where(t => AttributeExtensions.GetAttribute<ManuallySearchedAttribute>(t) == null)
                    .Select(t => {
                        List<ExpressionToken> DetermineSwallows(ExpressionToken tkn, List<ExpressionToken> @return) {
                            @return = @return ?? new List<ExpressionToken>();
                            List<ExpressionToken> swlws = tkn.GetAttributes<SwallowsAttribute>().SelectMany(sw => sw.Targets).Distinct().ToList();
                            foreach (var set in swlws) {
                                @return.Add(set);
                                return DetermineSwallows(set, @return);
                            }

                            return @return;
                        }

                        var attr = AttributeExtensions.GetAttribute<ExpressionTokenAttribute>(t);
                        if (attr == null)
                            return default;
                        var swallows = DetermineSwallows(t, null).OrderBy(tkn => AttributeExtensions.GetAttribute<ExpressionTokenAttribute>(tkn).Order).ToList();
                        return (attr.Regex, t, swallows, attr.Order);
                    })).Where(v => v != default && !exceptFor.Contains(v.Token));
            var allMatches = allTokens.SelectMany(
                    tkn => Regex.Matches(code.ToString(), tkn.Regex, Regexes.DefaultRegexOptions)
                        .Cast<Match>()
                        .Select(m => (tkn.Token, tkn.Swallows, tkn.Order, Match: m)))
                .OrderBy(t => t.Match.Index).ThenBy(t => t.Order).Select(t => new TokenDeletionMark(t))
                .ToList();

            if (allMatches.Count == 0) {
                //if it hit here, no matches were found at all.
                throw new ExpressionCompileException($"ExpressionLexer has stalled at '{code.ToString().Substring(0, Math.Min(15, code.Length))}...' (not incl. dots) and wasn't able to figure next token.");
            }

            var walker = allMatches.WrapWalker();
            do {
                _no_next:
                var curr = walker.Current;
                if (curr.Value.Token == ExpressionToken.Whitespace) {
                    curr.Delete = true;
                } else {
                    if (!walker.Next())
                        break;
                    var skipped = walker.SkipForwardWhile(nextToken => {
                        var shouldSwallow = (curr.Value.Match.IsMatchNestedTo(nextToken.Value.Match)) || nextToken.Value.Token == ExpressionToken.Whitespace;
                        nextToken.Delete = shouldSwallow;
                        return shouldSwallow;
                    });

                    if (skipped > 0)
                        goto _no_next;
                    else
                        walker.Back();
                }
            } while (walker.Next());


            //foreach (var t in allMatches.Where(t => !t.Delete && t.Value.Token != ExpressionToken.Whitespace)) {
            //    Console.WriteLine($"{t.Value.Token}: {t.Value.Match.Value}"); //\t\tSwallows: {string.Join(", ", t.Value.Swallows.Select(tk => tk.ToString()))}
            //}

            var ret = allMatches.Select(t => (Token: new TokenMatch(t.Value.Token, t.Value.Match), Data: t)).ToList();
            var retwalker = ret.WrapWalker();
            do {
                var curr = retwalker.Current;
                if (curr.Data.Value.Token == ExpressionToken.Whitespace)
                    continue;
                using (retwalker.CheckPoint()) {
                    retwalker.Next();
                    curr.Token.WhitespacesAfterMatch = retwalker.TakeForwardWhile(e => e.Data.Value.Token == ExpressionToken.Whitespace).Sum(m => m.Data.Value.Match.Value.Length);
                }
            } while (retwalker.Next());

            return ret.Where(r => !r.Data.Delete).Select(r => r.Token).ToList();
        }

        [DebuggerDisplay("{Value.Token} - {Value.Match} | {Delete}")]
        private class TokenDeletionMark {
            public (ExpressionToken Token, List<ExpressionToken> Swallows, int Order, Match Match) Value;
            public bool Delete;


            public TokenDeletionMark((ExpressionToken Token, List<ExpressionToken> Swallows, int Order, Match Match) value) {
                Value = value;
            }
        }
        //public static List<EToken> Tokenize(string code) {
        //    var possabilities = new List<(string Regex, ExpressionToken Token)>(
        //        Enum.GetValues(typeof(ExpressionToken)).Cast<ExpressionToken>()
        //            .Where(t => t.GetAttribute<ManuallySearchedAttribute>() == null)
        //            .Select(t => (t.GetAttribute<DescriptionAttribute>().Description, t)));

        //    var results = possabilities.Select(key => (poss: key, findings: Regex.Matches(code, key.Regex, Regexes.DefaultRegexOptions)))
        //        .SelectMany(found => found.findings.Cast<Match>().Select(m => (Token: found.poss.Token, Match: m)))
        //        .OrderBy(m => {
        //            var indx = m.Match.Groups.Count > 1 ? m.Match.Groups.Cast<Group>().Skip(1).Select(g => g.Length).Max() : 0;
        //            return m.Match.Index + indx;
        //        })
        //        .Select(t => new EToken(t.Token, t.Match))
        //        .ToList();

        //    return results;
        //}

        public static List<TokenMatch> FindSpecificToken(string code, ExpressionToken token) {
            var regex = AttributeExtensions.GetAttribute<DescriptionAttribute>(token).Description;

            return Regex.Matches(code, regex, Regexes.DefaultRegexOptions)
                .Cast<Match>().Select(m => new TokenMatch(token, m))
                .ToList();
        }

        public static List<TokenMatch> FindSpecificTokens(string codeStr, params ExpressionToken[] tokens) {
            var code = new StringBuilder(codeStr);
            var allTokens = new List<(string Regex, ExpressionToken Token, List<ExpressionToken> Swallows, int Order)>(
                tokens.Where(t => AttributeExtensions.GetAttribute<ManuallySearchedAttribute>(t) == null)
                    .Select(t => {
                        List<ExpressionToken> DetermineSwallows(ExpressionToken tkn, List<ExpressionToken> @return) {
                            @return = @return ?? new List<ExpressionToken>();
                            List<ExpressionToken> swlws = tkn.GetAttributes<SwallowsAttribute>().SelectMany(sw => sw.Targets).Distinct().ToList();
                            foreach (var set in swlws) {
                                @return.Add(set);
                                return DetermineSwallows(set, @return);
                            }

                            return @return;
                        }

                        var attr = AttributeExtensions.GetAttribute<ExpressionTokenAttribute>(t);
                        if (attr == null)
                            return default;
                        var swallows = DetermineSwallows(t, null).OrderBy(tkn => AttributeExtensions.GetAttribute<ExpressionTokenAttribute>(tkn).Order).ToList();
                        return (attr.Regex, t, swallows, attr.Order);
                    })).Where(v => v != default);

            var allMatches = allTokens.SelectMany(
                    tkn => Regex.Matches(code.ToString(), tkn.Regex, Regexes.DefaultRegexOptions)
                        .Cast<Match>()
                        .Select(m => (tkn.Token, tkn.Swallows, tkn.Order, Match: m)))
                .OrderBy(t => t.Match.Index).ThenBy(t => t.Order).Select(t => new TokenDeletionMark(t))
                .ToList();

            if (allMatches.Count == 0) {
                //if it hit here, no matches were found at all.
                throw new ExpressionCompileException($"ExpressionLexer has stalled at '{code.ToString().Substring(0, Math.Min(15, code.Length))}...' (not incl. dots) and wasn't able to figure next token.");
            }

            var walker = allMatches.WrapWalker();
            do {
                _no_next:
                var curr = walker.Current;
                if (curr.Value.Token == ExpressionToken.Whitespace) {
                    curr.Delete = true;
                } else {
                    if (!walker.Next())
                        break;
                    var skipped = walker.SkipForwardWhile(nextToken => {
                        var shouldSwallow = (curr.Value.Match.IsMatchNestedTo(nextToken.Value.Match)) || nextToken.Value.Token == ExpressionToken.Whitespace;
                        nextToken.Delete = shouldSwallow;
                        return shouldSwallow;
                    });

                    if (skipped > 0)
                        goto _no_next;
                    else
                        walker.Back();
                }
            } while (walker.Next());


            //foreach (var t in allMatches.Where(t => !t.Delete && t.Value.Token != ExpressionToken.Whitespace)) {
            //    Console.WriteLine($"{t.Value.Token}: {t.Value.Match.Value}"); //\t\tSwallows: {string.Join(", ", t.Value.Swallows.Select(tk => tk.ToString()))}
            //}

            var ret = allMatches.Select(t => (Token: new TokenMatch(t.Value.Token, t.Value.Match), Data: t)).ToList();
            var retwalker = ret.WrapWalker();
            do {
                var curr = retwalker.Current;
                if (curr.Data.Value.Token == ExpressionToken.Whitespace)
                    continue;
                using (retwalker.CheckPoint()) {
                    retwalker.Next();
                    curr.Token.WhitespacesAfterMatch = retwalker.TakeForwardWhile(e => e.Data.Value.Token == ExpressionToken.Whitespace).Sum(m => m.Data.Value.Match.Value.Length);
                }
            } while (retwalker.Next());

            return ret.Where(r => !r.Data.Delete).Select(r => r.Token).ToList();
        }

        public static List<TokenMatch> FindRegex(string codeStr, string regex) {
            var code = new StringBuilder(codeStr);
            var allTokens = new List<(string Regex, ExpressionToken Token, List<ExpressionToken> Swallows, int Order)>() {(regex, ExpressionToken.None, new List<ExpressionToken>(), 0)};

            var allMatches = allTokens.SelectMany(
                    tkn => Regex.Matches(code.ToString(), tkn.Regex, Regexes.DefaultRegexOptions)
                        .Cast<Match>()
                        .Select(m => (tkn.Token, tkn.Swallows, tkn.Order, Match: m)))
                .OrderBy(t => t.Match.Index).ThenBy(t => t.Order).Select(t => new TokenDeletionMark(t))
                .ToList();

            if (allMatches.Count == 0) {
                //if it hit here, no matches were found at all.
                throw new ExpressionCompileException($"ExpressionLexer has stalled at '{code.ToString().Substring(0, Math.Min(15, code.Length))}...' (not incl. dots) and wasn't able to figure next token.");
            }

            var walker = allMatches.WrapWalker();
            do {
                _no_next:
                var curr = walker.Current;
                if (curr.Value.Token == ExpressionToken.Whitespace) {
                    curr.Delete = true;
                } else {
                    if (!walker.Next())
                        break;
                    var skipped = walker.SkipForwardWhile(nextToken => {
                        var shouldSwallow = (curr.Value.Match.IsMatchNestedTo(nextToken.Value.Match)) || nextToken.Value.Token == ExpressionToken.Whitespace;
                        nextToken.Delete = shouldSwallow;
                        return shouldSwallow;
                    });

                    if (skipped > 0)
                        goto _no_next;
                    else
                        walker.Back();
                }
            } while (walker.Next());


            var ret = allMatches.Select(t => (Token: new TokenMatch(t.Value.Token, t.Value.Match), Data: t)).ToList();
            var retwalker = ret.WrapWalker();
            do {
                var curr = retwalker.Current;
                if (curr.Data.Value.Token == ExpressionToken.Whitespace)
                    continue;
                using (retwalker.CheckPoint()) {
                    retwalker.Next();
                    curr.Token.WhitespacesAfterMatch = retwalker.TakeForwardWhile(e => e.Data.Value.Token == ExpressionToken.Whitespace).Sum(m => m.Data.Value.Match.Value.Length);
                }
            } while (retwalker.Next());

            return ret.Where(r => !r.Data.Delete).Select(r => r.Token).ToList();
        }

        public static string ReplaceRegex(string codeStr, string regex, MatchEvaluator ev) {
            return Regex.Replace(codeStr, regex, ev, Regexes.DefaultRegexOptions);
        }
        public static string ReplaceRegex(string codeStr, string regex, string replaceWith) {
            return Regex.Replace(codeStr, regex, replaceWith, Regexes.DefaultRegexOptions);
        }
    }
}