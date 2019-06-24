using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Compiler;
using Regen.Compiler.Expressions;

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
                .Contain("2");
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
                .Contain("2");
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
                .Contain("2");
        }

        [TestMethod]
        public void foreach_using_builtin__vars__() {
            var @output = @"
                %a = [1,2,3]
                %b = ""hey""";

            var @input = $@"
                {@output}
                %foreach __vars__.keys(),__vars__.values()%
                    #1 = #2
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
                    case ""#1"": 123123
                        return _array#1;
                %
                ";
            Compile(@input).Output.Should()
                .Contain("_array1").And
                .Contain("_array2").And
                .Contain("_array3").And
                .Contain("_array4").And
                .Contain("_array5").And
                .Contain(@"""1"": 123123").And
                .Contain(@"""2"": 123123").And
                .Contain(@"""3"": 123123").And
                .Contain(@"""4"": 123123").And
                .Contain(@"""5"": 123123");
        }
    }
}