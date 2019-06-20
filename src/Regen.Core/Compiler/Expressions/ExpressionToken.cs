using System;
using System.ComponentModel;

namespace Regen.Compiler.Expressions {
    public enum ExpressionToken {
        [Description(@"import")] Import,
        [Description(@"function")] Function,
        [Description(@"if")] If,
        [Description(@"else ?if")] ElseIf,
        [Description(@"else")] Else,
        [Description(@"foreach")] Foreach,
        [Description(@"return")] Return,
        [Description(@"var")] Declaration,
        [Description(@"   \"".*?\""   ")] StringLiteral,
        [Description(@"[0-9]+(?:\.[0-9]+)?")] NumberLiteral, //"[0-9][0-9]*"
        [Description(@"[a-zA-Z_][a-zA-Z0-9_]*")] Identity,
        [Description(@"\//")] CommentRow,
        [Description(@"\%")] MARKER,
        [Description(@"[ \t]+")] Whitespace,
        [Description(@"\n")] NewLine,
        [Description(@"\+")] Add,
        [Description(@"\-")] Sub,
        [Description(@"\*")] Mul,
        [Description(@"\/")] Div,
        [Description(@"\==")] DoubleEqual,
        [Description(@"\!=")] NotEqual,
        [Description(@"\=")] Equal,
        [Description(@"\(")] LeftParen,
        [Description(@"\)")] RightParen,
        [Description(@"\{")] LeftBrackets,
        [Description(@"\}")] RightBrackets,
        [Description(@"\,")] Comma,
        [Description(@"\.")] Period,
        [Description(@"\?")] QuestionMark,
        [Description(@"\:")] Colon,
        [Description(@"\@")] Lambda,

    }


    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class ExpressionTokenAttribute : Attribute {
        public string Regex { get; }

        public ExpressionTokenAttribute(string regex) {
            Regex = regex;
        }
    }
}