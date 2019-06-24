using System.Collections.Generic;
using Regen.Collections;
using Regen.Compiler.Digest;
using Regen.Compiler.Expressions;
using Regen.Compiler.Helpers;
using Regen.DataTypes;

namespace Regen.Compiler {
    public class ParsedCode {
        public string OriginalCode { get; set; }
        public LineBuilder Output { get; set; }
        public Dictionary<string, object> Variables { get; set; }

        public List<EToken> ETokens { get; set; }
        public OList<ParserAction> ParseActions { get; set; }
        public InterpreterOptions Options { get; set; }
    }
}