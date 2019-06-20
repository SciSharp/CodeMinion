using System;
using System.ComponentModel;

namespace Regen.Compiler {
    public enum ExpressionToken {
        [Description(@"import")] Import,
        [Description(@"function")] Function,
        [Description(@"if")] If,
        [Description(@"else ?if")] ElseIf,
        [Description(@"else")] Else,
        [Description(@"foreach")] Foreach,
        [Description(@"return")] Return,
        [Description(@"   \"".*?\""   ")] StringLiteral,
        [Description(@"[0-9]+(?:\.[0-9]+)?")] NumberLiteral, //"[0-9][0-9]*"
        [Description(@"[a-zA-Z_][a-zA-Z0-9_]*")] Identity,
        [Description(@"[ \t]+")] Whitespace,
        [Description(@"\n")] NewLine,
        [Description(@"\+")] Add,
        [Description(@"\-")] Sub,
        [Description(@"\*")] Mul,
        [Description(@"\/")] Div,
        [Description(@"\==")] DoubleEqual,
        [Description(@"\!=")] NotEqual,
        [Description(@"\=")] Equal,
        [Description(@"\(")] LeftBrace,
        [Description(@"\)")] RightBrace,
        [Description(@"\{")] LeftBrackets,
        [Description(@"\}")] RightBrackets,
        [Description(@"\,")] Comma,
        [Description(@"\.")] Period,
        [Description(@"\@")] Lambda,
        [Description(@"\%//")] CommentRow,
        [Description(@"\%")] Block,
    }
}



[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
sealed class ExpressionTokenAttribute : Attribute {
    public string Regex { get; }

    public ExpressionTokenAttribute(string regex) {
        Regex = regex;
    }
}

