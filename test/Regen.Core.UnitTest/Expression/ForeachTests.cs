using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Compiler;
using Regen.Compiler.Expressions;
using Regen.Parser.Expressions;

namespace Regen.Core.Tests.Expression {
    [TestClass]
    public class ForeachTests : ExpressionUnitTest {
        //todo test bad foreach expressions
        //todo test nested foreach expressions
        //todo test removal of the expression, here and everywhere else. test that after compile they dont contain % etc..

        [TestMethod]
        public void foreach_i_multiline() {
            var @input = @"
                %a = [1,2,3]
                %foreach a%
                    #(i)
                %
                ";
            Compile(@input).Output.Should()
                .Contain("0").And
                .Contain("1").And
                .Contain("2").And
                .NotContain("%");
        }

        [TestMethod]
        public void foreach_i_singleline() {
            var @input = @"
                %a = [1,2,3]
                %foreach a
                    #(i)
                ";
            Compile(@input).Output.Should()
                .Contain("0").And
                .Contain("1").And
                .Contain("2").And
                .NotContain("%");
        }

        [TestMethod]
        public void foreach_missing_endblock_implicit_EOF() {
            var @input = @"
                %a = [1,2,3]
                %foreach a%
                    #(i)
                ";
            Compile(@input).Output.Should()
                .Contain("0").And
                .Contain("1").And
                .Contain("2").And
                .NotContain("%");
        }

        [TestMethod]
        public void foreach_using_builtin__vars__() {
            var @output = @"
                %a = 3
                %b = ""hey""";

            var @input = $@"
                {@output}
                %foreach __vars__.keys(),__vars__.values()%
                    %#1 = #2
                ";

            //todo to make this work we need to find a way to shrink references (see how the output looks) / garbage collect 
            Compile(@input).Output
                .Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "")
                .Should()
                .BeEquivalentTo(output.Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace("\"", ""));
        }

        [TestMethod]
        public void foreach_hashtags_inside_stringliterals() {
            var @input = @"
                %foreach range(3)%
                    Console.WriteLine(""#(#1*10)""+""Printed #1!"");
                %
                ";
            Compile(@input).Output.Should()
                .Contain("\"0\"+\"Printed 0").And
                .Contain("\"10\"+\"Printed 1").And
                .Contain("\"20\"+\"Printed 2");
        }

        [TestMethod]
        public void foreach_multiline_emittion() {
            var @input = @"
                %a = [1,2,3,4,5]
                %foreach a%
                    #g = 5
                    \#g = 5
                    #(g)
                    case ""#1"": 123123
                        return _array#1;
                %
                ";

            Compile(@input).Output.Should()
                .ContainAll("_array1", "_array2", "_array3", "_array4", "_array5",
                    @"""1"": 123123", @"""2"": 123123", @"""3"": 123123", @"""4"": 123123", @"""5"": 123123", @"""5"": 123123",
                    "#g = 5");
        }


        [TestMethod]
        public void foreach_longest() {
            var @input = @"
                %a = [1,2,3]
                %b = [1,2,3,4,5]
                %foreach longest a,b%
                    #1 #2
                %
                ";
            Compile(@input).Output.Should()
                .Contain("4").And
                .Contain("5");
        }

        [TestMethod]
        public void foreach_nested_expression() {
            var @input = @"
                %foreach range(3)%
                    Console.WriteLine(""#(#1*10)""+""Printed #1!"");
                %
                ";
            Compile(@input).Output.Should()
                .Contain("\"0\"+\"Printed 0").And
                .Contain("\"10\"+\"Printed 1").And
                .Contain("\"20\"+\"Printed 2");
        }

        [TestMethod]
        public void foreach_nested_foreaches() {
            var @input = @"
                %foreach range(3)%
                    #a = 5
                    %foreach range(3)%
                        //#(#1+#101)
                    %
                    Console.WriteLine("""");
                %
                ";
            Compile(@input).Output.Should().ContainAll("//4", "//1");
        }

        [TestMethod]
        public void foreach_nested_foreaches_three() {
            var @input = @"
                %foreach [1,2,3]%
                    %foreach [4,5,6]%
                        %foreach [7,8,9]%
                            //vals:  #1 + #101 + #201   |   i:  #(i + i1 + i2)
                        %
                    %
                %
                %a = 1
                ";
            Compile(@input).Output.Should().ContainAll("i:  6", "//vals:  3 + 6 + 9", "//vals:  1 + 4 + 7");
        }

        [TestMethod]
        public void foreach_nested_foreaches_variables() {
            var @input = @"
                %foreach [1,2,3]%
                    #a = [1,2,#1]
                    %foreach a%
                        %foreach [7,8,9]%
                            //vals:  #1 + #101 + #201   |   i:  #(i + i1 + i2)
                        %
                    %
                %
                %a = 1
                ";
            Compile(@input); //.Output.Should().ContainAll("i:  6", "//vals:  3 + 6 + 9", "//vals:  1 + 4 + 7");
        }

        [TestMethod]
        public void foreach_find_closer_nested() {
            var @input = @"%
                    #a = 5
                    %foreach range(3)%
                    //#(#1+#101)
                    %
                    Console.WriteLine("");
                %
                ";

            ForeachExpression.FindCloser(1, input).Should().Be(187);
        }

        [TestMethod]
        public void foreach_find_closer_nonnested() {
            var @input = @"%
                    Console.WriteLine("");
                %
                ";
            ForeachExpression.FindCloser(1, input).Should().Be(62);
        }
    }
}