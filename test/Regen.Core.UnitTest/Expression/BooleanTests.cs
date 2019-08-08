using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Compiler;
using Regen.Compiler.Expressions;
using Regen.Parser.Expressions;

namespace Regen.Core.Tests.Expression {
    [TestClass]
    public class BooleanTests : ExpressionUnitTest {
        [TestMethod]
        public void equals_case1() {
            var @input = @"
                %a = 1 == 1
                %(a)
                ";

            Compile(@input).Output.Should()
                .Contain("true");
        }

        [TestMethod]
        public void equals_case2() {
            var @input = @"
                %r = 1
                %a = 1 == r
                %(a)
                ";

            Compile(@input).Output.Should()
                .Contain("true");
        }

        [TestMethod]
        public void equals_case3() {
            var @input = @"
                %l = 1
                %a = l == 1
                %(a)
                ";

            Compile(@input).Output.Should()
                .Contain("true");
        }

        [TestMethod]
        public void equals_case4() {
            var @input = @"
                %l = [1]
                %a = l == 1
                %(a)
                ";

            Compile(@input).Output.Should()
                .Contain("false");
        }


        [TestMethod]
        public void equals_case5() {
            var @input = @"
                %a = False == True
                %(a)
                ";

            Compile(@input).Output.Should()
                .Contain("false");
        }


        [TestMethod]
        public void equals_case6() {
            var @input = @"
                %a = True == False
                %(a)
                ";

            Compile(@input).Output.Should()
                .Contain("false");
        }

        [TestMethod]
        public void equals_case7() {
            var @input = @"
                %a = True == True
                %(a)
                ";

            Compile(@input).Output.Should()
                .Contain("true");
        }

        [TestMethod]
        public void equals_case7_lowercase() {
            var @input = @"
                %a = true == true
                %(a)
                ";

            Compile(@input).Output.Should()
                .Contain("true");
        }

        [TestMethod]
        public void equals_case8() {
            var @input = @"
                %a = False == False
                %(a)
                ";

            Compile(@input).Output.Should()
                .Contain("true");
        }

        [TestMethod]
        public void equals_case9() {
            var @input = @"
                %a = False || True
                %(a)
                ";

            Compile(@input).Output.Should()
                .Contain("true");
        }

        [TestMethod]
        public void equals_case10() {
            var @input = @"
                %a = False | True
                %(a)
                ";

            Compile(@input).Output.Should()
                .Contain("true");
        }

        [TestMethod]
        public void equals_case11() {
            var @input = @"
                %a = False & True
                %(a)
                ";

            Compile(@input).Output.Should()
                .Contain("false");
        }

        [TestMethod]
        public void equals_case12() {
            var @input = @"
                %a = False && True
                %(a)
                ";

            Compile(@input).Output.Should()
                .Contain("false");
        }

        [TestMethod]
        public void equals_case13() {
            var @input = @"
                %a = 1.2 == 1.2
                %(a)
                ";

            Compile(@input).Output.Should()
                .Contain("true");
        }

        [TestMethod]
        public void equals_case13_approx() {
            var @input = @"
                %a = 1.2 ~= 1.2
                %(a)
                ";

            Compile(@input).Output.Should()
                .Contain("true");
        }

        [TestMethod]
        public void equals_case13_approx2() {
            var @input = @"
                %a = 1.2 ~= 1.1
                %(a)
                ";

            Compile(@input).Output.Should()
                .Contain("false");
        }

        [TestMethod]
        public void equals_case14() {
            var @input = @"
                %a = !false
                %(a)
                ";

            Compile(@input).Output.Should()
                .Contain("true");
        }

        [TestMethod]
        public void equals_case15() {
            var @input = @"
                %bo = false
                %a = !bo
                %(a)
                ";

            Compile(@input).Output.Should()
                .Contain("true");
        }

        [TestMethod]
        public void equals_case16() {
            var @input = @"
                %bo = false
                %a = !true
                %(a)
                ";

            Compile(@input).Output.Should()
                .Contain("false");
        }

        [TestMethod]
        public void devvv() {
            var @input = @"
                %bo = false
                %a = !bo
                %(a)
                ";

            Compile(@input).Output.Should()
                .Contain("true");
        }
    }
}