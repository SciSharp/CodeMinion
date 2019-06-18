using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Regen.Compiler {
    public static class Lexer {
        public static List<Token> Tokenize(string code) {
            var possabilities = new List<(string Regex, TokenID Token)>(
                Enum.GetValues(typeof(TokenID)).Cast<TokenID>()
                    .Where(t => t.GetAttribute<ManuallySearchedAttribute>() == null)
                    .Select(t => (t.GetAttribute<DescriptionAttribute>().Description, t)));

            var results = possabilities.Select(key => (poss: key, findings: Regex.Matches(code, key.Regex, Regexes.DefaultRegexOptions)))
                .SelectMany(found => found.findings.Cast<Match>().Select(m => (Token: found.poss.Token, Match: m)))
                .OrderBy(m => {
                    var indx = m.Match.Groups.Count > 1 ? m.Match.Groups.Cast<Group>().Skip(1).Select(g => g.Length).Max() : 0;
                    return m.Match.Index + indx;
                })
                .Select(t => new Token(t.Token, t.Match))
                .ToList();

            return results;
        }

        public static List<Token> FindTokens(TokenID tokenId, string code) {
            var regex = tokenId.GetAttribute<DescriptionAttribute>().Description;

            return Regex.Matches(code, regex, Regexes.DefaultRegexOptions)
                .Cast<Match>().Select(m => new Token(tokenId, m))
                .ToList();
        }
    }
}