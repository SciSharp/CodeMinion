using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Compiler;

namespace Regen.Core.Tests {
    [TestClass]
    public class BuiltinTests : UnitTestBase {
        [TestMethod]
        public void range_2() {
            var @input = @"

                %foreach range(3,3)%
                    Console.WriteLine(""Printed #1!"");
                %
                ";

            Interpert(input)
                .Should()
                .Contain("Printed 3").And
                .Contain("Printed 4").And
                .Contain("Printed 5");
        }

        [TestMethod]
        public void range_1() {
            var @input = @"
                %foreach range(3)%
                    Console.WriteLine(""Printed #1!"");
                %
                ";
            Interpert(@input).Should()
                .Contain("Printed 0").And
                .Contain("Printed 1").And
                .Contain("Printed 2");
        }

        [TestMethod]
        public void zipmax() {
            var @input = @"
                %a = [1|2|3]
                %b = [1|2|3|4|5]
                %foreach zipmax(a,b)%
                    #1 #2
                %
                ";
            Interpert(@input).Should()
                .Contain("4").And
                .Contain("5");
        }

        [TestMethod]
        public void zipmax_overload_ziplongest() {
            var @input = @"
                %a = [1|2|3]
                %b = [1|2|3|4|5]
                %foreach ziplongest(a,b)%
                    #1 #2
                %
                ";
            Interpert(@input).Should()
                .Contain("4").And
                .Contain("5");
        }

        [TestMethod]
        public void without_zipmax() {
            var @input = @"
                %a = [1|2|3]
                %b = [1|2|3|4|5]
                %foreach a|b%
                    #1 #2
                %
                ";
            Interpert(@input).Should()
                .Contain("1").And
                .Contain("2").And
                .Contain("3").And
                .NotContain("4").And
                .NotContain("5");
        }

    }
}