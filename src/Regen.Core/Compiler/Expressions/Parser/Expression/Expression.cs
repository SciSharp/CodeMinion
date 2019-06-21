using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Regen.Compiler.Expressions {
    public class Expression {
        public static EmptyExpression None { get; } = new EmptyExpression();

        public virtual IEnumerable<Match> Matches() {
            yield break;
        }
    }

    public class EmptyExpression : Expression { }
}