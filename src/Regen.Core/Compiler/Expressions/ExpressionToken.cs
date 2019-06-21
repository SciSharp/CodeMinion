﻿using System.ComponentModel;

namespace Regen.Compiler.Expressions {
    public enum ExpressionToken {
        [ExpressionToken(@"\//", -1)] [Swallows(Div)] CommentRow, //swallow will 
        [ExpressionToken(@"import", 0)] [Swallows(StringLiteral)] Import,
        [ExpressionToken(@"as", 1)] [Swallows(StringLiteral)] As,
        [ExpressionToken(@"function", 5)] [Swallows(StringLiteral)] Function,
        [ExpressionToken(@"new", 8)] [Swallows(StringLiteral)] New,
        [ExpressionToken(@"throw", 9)] [Swallows(StringLiteral)] Throw,
        [ExpressionToken(@"if", 10)] [Swallows(StringLiteral)] If,
        [ExpressionToken(@"else\s?if", 15)] [Swallows(StringLiteral)] ElseIf,
        [ExpressionToken(@"else", 20)] [Swallows(StringLiteral)] Else,
        [ExpressionToken(@"(true|false)", 21)] [Swallows(StringLiteral)] Boolean,
        [ExpressionToken(@"foreach", 25)] [Swallows(StringLiteral)] Foreach,
        [ExpressionToken(@"while", 26)] [Swallows(StringLiteral)] While,
        [ExpressionToken(@"do", 27)] [Swallows(StringLiteral)] Do,
        [ExpressionToken(@"reset", 28)] [Swallows(StringLiteral)] Reset,
        [ExpressionToken(@"return", 30)] [Swallows(StringLiteral)] Return,
        [ExpressionToken(@"case", 31)] [Swallows(StringLiteral)] Case,
        [ExpressionToken(@"switch", 32)] [Swallows(StringLiteral)] Switch,
        [ExpressionToken(@"break", 33)] [Swallows(StringLiteral)] Break,
        [ExpressionToken(@"continue", 34)] [Swallows(StringLiteral)] Continue,
        [ExpressionToken(@"default", 35)] [Swallows(StringLiteral)] Default,
        [ExpressionToken(@"var", 39)] [Swallows(StringLiteral)] Declaration,
        [ExpressionToken(@"\'(.|\\.)\'", 40)] [Swallows(StringLiteral)] CharLiteral,
        [ExpressionToken(@"\""(.*?)\""", 41)] [Swallows(NumberLiteral)] StringLiteral,
        [ExpressionToken(@"[0-9]+(?:\.[0-9]+)?[fFdDmM]?", 45)] [Swallows(Literal)] NumberLiteral,
        [ExpressionToken(@"[a-zA-Z_][a-zA-Z0-9_]*", 50)] Literal,
        [ExpressionToken(@"[\s\t]+", 60)] Whitespace,
        [ExpressionToken(@"\n", 65)] NewLine,
        [ExpressionToken(@"\r", 70)] UnixNewLine,
        [ExpressionToken(@"\%", 75)] MARKER,
        [ExpressionToken(@"\+\+", 80)] [Swallows(Add)] Increment,
        [ExpressionToken(@"\-\-", 85)] [Swallows(Sub)] Decrement,
        [ExpressionToken(@"\+", 90)] Add,
        [ExpressionToken(@"\-", 95)] Sub,
        [ExpressionToken(@"\*\*", 100)] [Swallows(Mul)] Pow,
        [ExpressionToken(@"\*", 105)] Mul,
        [ExpressionToken(@"\/", 110)] Div,
        [ExpressionToken(@"\==", 115)] [Swallows(Equal)] DoubleEqual,
        [ExpressionToken(@"\!=", 120)] [Swallows(Equal)] NotEqual,
        [ExpressionToken(@"\=", 125)] Equal,
        [ExpressionToken(@"\&\&", 130)] [Swallows(And)] DoubleAnd,
        [ExpressionToken(@"\&", 135)] And,
        [ExpressionToken(@"\|\|", 140)] [Swallows(Or)] DoubleOr,
        [ExpressionToken(@"\|", 145)] Or,
        [ExpressionToken(@"\~", 150)] Not,
        [ExpressionToken(@"\!", 151)] NotBoolean,
        [ExpressionToken(@"\^", 155)] Xor,
        [ExpressionToken(@"\>\>", 160)] [Swallows(BiggerOrEqualThat)] ShiftRight,
        [ExpressionToken(@"\>\=", 165)] [Swallows(BiggerThan)] BiggerOrEqualThat,
        [ExpressionToken(@"\>", 170)] BiggerThan,
        [ExpressionToken(@"\<\<", 175)] [Swallows(SmallerOrEqualThat)] ShiftLeft,
        [ExpressionToken(@"\<\=", 180)] [Swallows(SmallerThan)] SmallerOrEqualThat,
        [ExpressionToken(@"\<", 185)] SmallerThan,
        [ExpressionToken(@"\(", 190)] LeftParen,
        [ExpressionToken(@"\)", 195)] RightParen,
        [ExpressionToken(@"\{", 200)] LeftBrace,
        [ExpressionToken(@"\}", 205)] RightBrace,
        [ExpressionToken(@"\[", 210)] LeftBracet,
        [ExpressionToken(@"\]", 215)] RightBracet,
        [ExpressionToken(@"\#", 220)] Hashtag,
        [ExpressionToken(@"\,", 225)] Comma,
        [ExpressionToken(@"\.\.", 230)] [Swallows(Period)] RangeTo,
        [ExpressionToken(@"\.", 235)] Period,
        [ExpressionToken(@"\?\?", 240)] [Swallows(QuestionMark)] NullCoalescing,
        [ExpressionToken(@"\?", 245)] QuestionMark,
        [ExpressionToken(@"\:", 250)] Colon,
        [ExpressionToken(@"\;", 255)] SemiColon,
        [ExpressionToken(@"\@", 260)] Lambda,
    }
}