using System.Collections.Generic;
using Regen.Collections;
using Regen.Compiler.Expressions;
using Regen.DataTypes;

namespace Regen.Compiler {
    public class InterpredCode {
        public string OriginalCode { get; set; }
        public string Output { get; set; }
        public Dictionary<string, object> Variables { get; set; }

        public List<EToken> ETokens { get; set; }
        public OList<ParserAction> ParseActions { get; set; }
    }
}