using System.ComponentModel;

namespace Regen.Compiler {
    public enum TokenID {
        [Description(@"%(\w+)(?=\s*=)")] Declaration,
        [Description(@"(?<=[=,\s])\[  (.*)  \]")]Array,
        [Description(@"(?<!for(?:each))(?:(?<=[=])\s+?  (?!\[)(.*) (?=$|\n|\r))")] Scalar,
        [Description(@"(?<!\\)\%\(([\s|\S]*?)[\)]")] Expression,
        [Description(@"%for(?:each)?\s")] ForEach,
        //dropped for loops [Description(@"(?:%for(?:each)?\s){1} ([^$\r\n%]+)")] ForExpression,
        [Description(@"(?<!\\)\#\((.*)\)")] [ManuallySearched] EmitExpression,
        [Description(@"(?<!\\)\#(\d+)(?!\[)")] [ManuallySearched] EmitVariable,
        [Description(@"(?<!\\)\#(\d+)\[(.*)]")] [ManuallySearched] EmitVariableOffsetted,
        [Description(@"%[\s|\t|\n|\r]")] BlockMark,
    }
}