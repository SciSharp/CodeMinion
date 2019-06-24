using System.Collections.Generic;
using Regen.Compiler.Expressions;
using Regen.Compiler.Helpers;
using Regen.DataTypes;
using Regen.Helpers.Collections;
using Regen.Parser;

namespace Regen.Compiler {
    public class ParsedCode {
        public string OriginalCode { get; set; }
        public LineBuilder Output { get; set; }
        public List<TokenMatch> ETokens { get; set; }
        public OList<ParserAction> ParseActions { get; set; }
        public InterpreterOptions Options { get; set; }
        public Dictionary<string, object> Variables { get; set; }
    }
}