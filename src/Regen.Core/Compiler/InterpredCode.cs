using System.Collections.Generic;
using Regen.DataTypes;

namespace Regen.Compiler {
    public class InterpredCode {
        public string OriginalCode { get; set; }
        public string Output { get; set; }
        public Dictionary<string, object> Variables { get; set; }
    }
}