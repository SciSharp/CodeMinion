using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Helpers.Collections;

namespace Regen.Core.Tests.Collections {
    [TestClass]
    public class TokenWalkerTests {
        public enum Token {
            A,
            B,
            C,
            D,
            E,
        }

        [TestMethod]
        public void IsNext() {
            List<Token> rawTokens;
            var tkns = TokenWalker.WrapWalker(rawTokens = new List<Token>() {Token.A, Token.B, Token.C, Token.D, Token.E});

            do {
                tkns.IsNext(rawTokens[tkns.Cursor + 1]).Should().BeTrue();
            } while (tkns.Next() && tkns.HasNext);
        }

        [TestMethod]
        public void IsBack() {
            List<Token> rawTokens;
            var tkns = TokenWalker.WrapWalker(rawTokens = new List<Token>() {Token.A, Token.B, Token.C, Token.D, Token.E});
            tkns.Next();
            do {
                tkns.IsBack(rawTokens[tkns.Cursor - 1]).Should().BeTrue();
            } while (tkns.Next());
        }


        [TestMethod]
        public void IsNext_goNext() {
            List<Token> rawTokens;
            var tkns = TokenWalker.WrapWalker(rawTokens = new List<Token>() {Token.A, Token.B, Token.C, Token.D, Token.E});
            tkns.Next(2);
            tkns.IsNext(Token.D, gotNextIfTrue: true).Should().BeTrue();
            tkns.IsNext(Token.E, gotNextIfTrue: true).Should().BeTrue();
            tkns.IsBack(Token.E, gotNextIfTrue: true).Should().BeFalse();
        }

        [TestMethod]
        public void IsNext_goBack() {
            List<Token> rawTokens;
            var tkns = TokenWalker.WrapWalker(rawTokens = new List<Token>() {Token.A, Token.B, Token.C, Token.D, Token.E});
            tkns.Next(2);
            tkns.IsNext(Token.D, gotNextIfTrue: true).Should().BeTrue();
            tkns.IsNext(Token.E, gotNextIfTrue: true).Should().BeTrue();
        }

        [TestMethod]
        public void IsBack_goNext() {
            List<Token> rawTokens;
            var tkns = TokenWalker.WrapWalker(rawTokens = new List<Token>() {Token.A, Token.B, Token.C, Token.D, Token.E});
            tkns.Next(2);
            tkns.IsBack(Token.B, gotNextIfTrue: true).Should().BeTrue();
            tkns.IsBack(Token.C, gotNextIfTrue: true).Should().BeTrue();
        }

        [TestMethod]
        public void IsBack_goBack() {
            List<Token> rawTokens;
            var tkns = TokenWalker.WrapWalker(rawTokens = new List<Token>() {Token.A, Token.B, Token.C, Token.D, Token.E});
            tkns.Next(2);
            tkns.IsBack(Token.B, goBackIfTrue: true).Should().BeTrue();
            tkns.IsBack(Token.A, goBackIfTrue: true).Should().BeTrue();
            tkns.IsBack(Token.A, goBackIfTrue: true).Should().BeFalse();
        }


        [TestMethod]
        public void AreNext() {
            List<Token> rawTokens;
            var tkns = TokenWalker.WrapWalker(rawTokens = new List<Token>() {Token.A, Token.B, Token.C, Token.D, Token.E});
            tkns.Next(2);
            tkns.AreNext(Token.D, Token.E).Should().BeTrue();
            tkns.AreNext(Token.D).Should().BeTrue();
            tkns.AreNext(Token.E).Should().BeFalse();
            tkns.Next(2);
            tkns.AreNext(Token.E).Should().BeFalse();
        }

        [TestMethod]
        public void AreBack() {
            List<Token> rawTokens;
            var tkns = TokenWalker.WrapWalker(rawTokens = new List<Token>() {Token.A, Token.B, Token.C, Token.D, Token.E});
            tkns.Next(2);
            tkns.AreBack(Token.B, Token.A).Should().BeTrue();
            tkns.AreBack(Token.B).Should().BeTrue();
            tkns.AreBack(Token.A).Should().BeFalse();
            tkns.Back(2);
            tkns.AreBack(Token.A).Should().BeFalse();
        }

        [TestMethod]
        public void SkipNext() {
            List<Token> rawTokens;
            var tkns = TokenWalker.WrapWalker(rawTokens = new List<Token>() {Token.A, Token.B, Token.C, Token.D, Token.E});
            tkns.Next(2);
            tkns.SkipNext(Token.D, Token.E).Should().BeTrue();
            tkns.HasNext.Should().BeFalse();
            tkns.Current.Should().Be(Token.E);
        }


        [TestMethod]
        public void SkipBack() {
            List<Token> rawTokens;
            var tkns = TokenWalker.WrapWalker(rawTokens = new List<Token>() {Token.A, Token.B, Token.C, Token.D, Token.E});
            tkns.Next(2);
            tkns.SkipBack(Token.B, Token.A).Should().BeTrue();
            tkns.HasBack.Should().BeFalse();
            tkns.Current.Should().Be(Token.A);
        }

        [TestMethod]
        public void ApplyOnlyIfTrue() {
            List<Token> rawTokens;
            var tkns = TokenWalker.WrapWalker(rawTokens = new List<Token>() {Token.A, Token.B, Token.C, Token.D, Token.E});
            tkns.Next(2);
            tkns.ApplyOnlyIfTrue(() => tkns.SkipBack(Token.B, Token.A, Token.A));
            tkns.HasBack.Should().BeTrue();
            tkns.Current.Should().Be(Token.C);
        }

        [TestMethod]
        public void ApplyOnlyIfTrue2() {
            List<Token> rawTokens;
            var tkns = TokenWalker.WrapWalker(rawTokens = new List<Token>() {Token.A, Token.B, Token.C, Token.D, Token.E});
            tkns.Next(2);
            tkns.ApplyOnlyIfTrue(() => tkns.SkipNext(Token.D, Token.E, Token.E)).Should().BeFalse();
            tkns.HasBack.Should().BeTrue();
            tkns.Current.Should().Be(Token.C);
        }
    }
}