using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Compiler;

namespace Regen.Core.Tests {
    [TestClass]
    public class ForeachTests : UnitTestBase {
        //todo test bad foreach expressions
        //todo test nested foreach expressions
        //todo test removal of the expression, here and everywhere else. test that after compile they dont contain % etc..


        [TestMethod]
        public void foreach_i_multiline() {
            var @input = @"
                %a = [1|2|3]
                %foreach a%
                    #(i)
                %
                ";
            Interpert(@input).Should()
                .Contain("0").And
                .Contain("1").And
                .Contain("2");
        }
        [TestMethod]
        public void foreach_i() {
            var @input = @"
                %a = [1|2|3]
                %foreach a
                    #(i)
                ";
            Interpert(@input).Should()
                .Contain("0").And
                .Contain("1").And
                .Contain("2");
        }
    }
}