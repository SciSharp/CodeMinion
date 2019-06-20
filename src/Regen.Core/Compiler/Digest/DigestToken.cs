using System.ComponentModel;

namespace Regen.Compiler.Digest {
    public enum DigestToken {
        [Description(@"%(\w+)(?=\s*=)")] Declaration,
        [Description(@"(?<=[\s=]) \[ (.*) \]")]Array,
        [Description(@"(?<!for(?:each))(?:(?<=[=])\s+?  (?!\[)(.*) (?=$|\n|\r))")] Scalar,
        [Description(@"(?<!\\)\%\(([\s|\S]*?)[\)](?=[$\s\n\r\b])")] Expression,
        [Description(@"%for(?:each)?\s")] ForEach,
        [Description(@"%import\s    ([\s\S]+?)   (?:\sas\s([\s\S]+?))?[\r\n]+")] Import,
        //dropped for loops [Description(@"(?:%for(?:each)?\s){1} ([^$\r\n%]+)")] ForExpression,
        [Description(@"(?<!\\)\#\(([\s|\S]*?)[\)]")] [ManuallySearched] EmitExpression,
        [Description(@"(?<!\\)\#(\d+)(?!\[)")] [ManuallySearched] EmitVariable,
        [Description(@"(?<!\\)\#(\d+)\[([\s|\S]*?)]  ")] [ManuallySearched] EmitVariableOffsetted,
        [Description(@"%[\s|\t|\n|\r]")] BlockMark,
        [Description(@"(?<!\\) (\#\/\/(?:[\s\S])+?)[\r\n]")] [ManuallySearched] CommentRow,
    }
}
