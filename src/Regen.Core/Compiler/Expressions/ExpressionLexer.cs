using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Regen.Compiler.Expressions {
    public class ExpressionLexer {
        public static List<EToken> Tokenize(string code) {
            var possabilities = new List<(string Regex, ExpressionToken Token)>(
                Enum.GetValues(typeof(ExpressionToken)).Cast<ExpressionToken>()
                    .Where(t => t.GetAttribute<ManuallySearchedAttribute>() == null)
                    .Select(t => (t.GetAttribute<DescriptionAttribute>().Description, t)));

            var results = possabilities.Select(key => (poss: key, findings: Regex.Matches(code, key.Regex, Regexes.DefaultRegexOptions)))
                .SelectMany(found => found.findings.Cast<Match>().Select(m => (Token: found.poss.Token, Match: m)))
                .OrderBy(m => {
                    var indx = m.Match.Groups.Count > 1 ? m.Match.Groups.Cast<Group>().Skip(1).Select(g => g.Length).Max() : 0;
                    return m.Match.Index + indx;
                })
                .Select(t => new EToken(t.Token, t.Match))
                .ToList();

            return results;
        }

        public static List<EToken> FinETokens(ExpressionToken digestToken, string code) {
            var regex = digestToken.GetAttribute<DescriptionAttribute>().Description;

            return Regex.Matches(code, regex, Regexes.DefaultRegexOptions)
                .Cast<Match>().Select(m => new EToken(digestToken, m))
                .ToList();
        }
    }
}