using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Compiler;
using Regen.DataTypes;

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

        [DataTestMethod]
        [DataRow(typeof(int), "1", "1")]
        [DataRow(typeof(double), "1.0d", "1")]
        [DataRow(typeof(double), "1.0D", "1")]
        [DataRow(typeof(float), "1.0f", "1")]
        [DataRow(typeof(float), "1.0F", "1")]
        [DataRow(typeof(decimal), "1.0m", "1.0")]
        [DataRow(typeof(decimal), "1.0M", "1.0")]
        [DataRow(typeof(long), "1L", "1")]
        public void str(Type type, string emit, string value) {
            var @input = $@"
                %a = str({emit})
                ";
            var variable = Variables(input).Values.First();
            variable.Should().BeOfType(typeof(StringScalar));
            variable.Value.As<string>().Should().BeEquivalentTo(value);
        }

        [DataTestMethod]
        [DataRow(typeof(double), "1.1d", "1.1")]
        [DataRow(typeof(double), "1.1D", "1.1")]
        [DataRow(typeof(float), "1.1f", "1.1")]
        [DataRow(typeof(float), "1.1F", "1.1")]
        [DataRow(typeof(decimal), "1.1m", "1.1")]
        [DataRow(typeof(decimal), "1.1M", "1.1")]
        public void str_with_point(Type type, string emit, string value) {
            var @input = $@"
                %a = str({emit})
                ";
            var variable = Variables(input).Values.First();
            variable.Should().BeOfType(typeof(StringScalar));
            variable.Value.As<string>().Should().BeEquivalentTo(value);
        }
    }
}