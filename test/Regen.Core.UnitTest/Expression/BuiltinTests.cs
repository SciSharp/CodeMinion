using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Compiler.Expressions;
using Regen.DataTypes;

namespace Regen.Core.Tests.Expression {
    [TestClass]
    public class BuiltinTests : ExpressionUnitTest {
        [TestMethod]
        public void range_1() {
            var @input = @"
                %foreach range(3)%
                    Console.WriteLine(""Printed #1!"");
                %
                ";
            Compile(@input).Output.Should()
                .Contain("Printed 0").And
                .Contain("Printed 1").And
                .Contain("Printed 2");
        }

        [TestMethod]
        public void zipmax() {
            var @input = @"
                %a = [1,2,3]
                %b = [1,2,3,4,5]
                %foreach zipmax(a,b)%
                    #1 #2
                %
                ";
            Compile(@input).Output.Should()
                .Contain("4").And
                .Contain("5");
        }

        [TestMethod]
        public void zipmax_overload_ziplongest() {
            var @input = @"
                %a = [1,2,3]
                %b = [1,2,3,4,5]
                %foreach ziplongest(a,b)%
                    #1 #2
                %
                ";
            Compile(@input).Output.Should()
                .Contain("4").And
                .Contain("5");
        }

        [TestMethod]
        public void without_zipmax() {
            var @input = @"
                %a = [1,2,3]
                %b = [1,2,3,4,5]
                %foreach a,b%
                    #1 #2
                %
                ";
            Compile(@input).Output.Should()
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
            var variable = Variables(input).Values.First()
                .Should().BeOfType<StringScalar>().Which;
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
            var variable = Variables(input).Values.First()
                .Should().BeOfType<StringScalar>().Which;
            variable.Value.As<string>().Should().BeEquivalentTo(value);
        }

        [TestMethod]
        public void len() {
            var @input = $@"
                %a = [1,2,3,4]
                %(len(a)+5)
                ";
            Compile(@input).Output.Should().Contain("9");
        }

        [TestMethod]
        public void forevery() {
            var @input = $@"
                %foreach forevery([1,2,3],[4,5,6],false)%
                    #1=#2
                %
                ";
            Compile(@input).Output.Trim(' ', '\t', '\n', '\r').Should()
                .Contain("1=4").And
                .Contain("1=5").And
                .Contain("1=6").And
                .Contain("2=4").And
                .Contain("2=5").And
                .Contain("2=6").And
                .Contain("3=4").And
                .Contain("3=5").And
                .Contain("3=6");
        }

        [TestMethod]
        public void forevery_with_exclusion() {
            var @input = $@"
                %foreach forevery([1,2,3],[3,4,5],true)%
                    #1=#2
                %
                ";
            Compile(@input).Output.Should()
                .Contain("1=3").And
                .Contain("1=4").And
                .Contain("1=5").And
                .Contain("2=3").And
                .Contain("2=4").And
                .Contain("2=5").And
                .Contain("3=4").And
                .Contain("3=5");
        }

        [TestMethod]
        public void forevery_with_exclusion_ofstring() {
            var @input = $@"
                %foreach forevery([1,2,3],[3,4,5],true)%
                    #1=#2
                %
                ";
            Compile(@input).Output.Should()
                .Contain("1=3").And
                .Contain("1=4").And
                .Contain("1=5").And
                .Contain("2=3").And
                .Contain("2=4").And
                .Contain("2=5").And
                .Contain("3=4").And
                .Contain("3=5");
        }

        [TestMethod]
        public void except() {
            var @input = $@"
                %foreach except([1,2,3], 1)%
                    #1
                %
                ";
            var ret = Compile(@input).Output
                .Should().ContainAll("2", "3").And.NotContain("1");
        }

        [TestMethod]
        public void except_string() {
            var @input = $@"
                %foreach except([""1"",""2"",""3""], ""2"")%
                    #1
                %
                ";
            var ret = Compile(@input).Output
                .Should().ContainAll("1", "3").And.NotContain("2");
        }

        [TestMethod]
        public void except_string_multiple_occurences() {
            var @input = $@"
                %foreach except([""2"",""2"",""3""], ""2"")%
                    #1
                %
                ";
            var ret = Compile(@input).Output
                .Should().ContainAll("3").And.NotContain("2");
        }

        [TestMethod]
        public void concat() {
            var @input = $@"
                %foreach concat([1],[2])%
                    #1
                %
                ";
            var ret = Compile(@input).Output
                .Should().ContainAll("1", "2");
        }

        [TestMethod]
        public void concat_3() {
            var @input = $@"
                %foreach concat([1],[2],[3,4])%
                    #1
                %
                ";
            var ret = Compile(@input).Output
                .Should().ContainAll("1", "2", "3", "4");
        }

        [TestMethod]
        public void flatten() {
            var @input = $@"
                %foreach flatten([1],[2,[3,4]],[[5],6])%
                    #1
                %
                ";
            var ret = Compile(@input).Output
                .Should().ContainAll("1", "2", "3", "4","5","6");
        }


        [TestMethod]
        public void repeat_normal()
        {
            var input = @"
                %(repeat(""a+1"", 2 ,  "",""  ,  ""[""  ,  """"  ,  """"  ,  ""]""  )))
                ";
            Compile(input).Output.Should().Contain("[a+1,a+1]");
        }

        [TestMethod]
        public void repeat_compile()
        {
            var input = @"
                %a = ""kek""
                %comma = "",""
                %foreach range(16)%
                //#(repeat(""^a+n"", i ,  ""^comma+n""  ,  ""[""  ,  """"  ,  """"  ,  ""]""  ))
                %
              ";
            var ret = base.Compile(input).Output.Should().Contain("[kek0,0kek1,1kek2,2kek3,3kek4,4kek5,5kek6,6kek7,7kek8,8kek9,9kek10,10kek11,11kek12,12kek13,13kek14]");
        }

        [TestMethod]
        public void join_skiptake()
        {
            var @input = @"
                %letters = [""a"",""b"",""c"",""d"",""e"",""f"",""g"",""h"",""i"",""j"",""k"",""l"",""m"",""n"",""o"",""p"",""q"",""r"",""s"",""t"",""u"",""v"",""w"",""x"",""y"",""z""]
                %(join("" - "", skiptake(letters, 3, 2)))
                ";

            Compile(@input).Output.Should().Contain("d - e");
        }

    }
}