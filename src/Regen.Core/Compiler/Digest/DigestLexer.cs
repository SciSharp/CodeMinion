using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Regen.Compiler.Helpers;

namespace Regen.Compiler.Digest {
    public static class DigestLexer {
        public static List<DToken> Tokenize(string code) {
            var possabilities = new List<(string Regex, DigestToken Token)>(
                Enum.GetValues(typeof(DigestToken)).Cast<DigestToken>()
                    .Where(t => t.GetAttribute<ManuallySearchedAttribute>() == null)
                    .Select(t => (t.GetAttribute<DescriptionAttribute>().Description, t)));

            var results = possabilities.Select(key => (poss: key, findings: Regex.Matches(code, key.Regex, Regexes.DefaultRegexOptions)))
                .SelectMany(found => found.findings.Cast<Match>().Select(m => (Token: found.poss.Token, Match: m)))
                .OrderBy(m => {
                    var indx = m.Match.Groups.Count > 1 ? m.Match.Groups.Cast<Group>().Skip(1).Select(g => g.Length).Max() : 0;
                    return m.Match.Index + indx;
                })
                .Select(t => new DToken(t.Token, t.Match))
                .ToList();

            return results;
        }

        public static List<DToken> FindTokens(DigestToken digestToken, string code) {
            var regex = digestToken.GetAttribute<DescriptionAttribute>().Description;

            return Regex.Matches(code, regex, Regexes.DefaultRegexOptions)
                .Cast<Match>().Select(m => new DToken(digestToken, m))
                .ToList();
        }
    }
}