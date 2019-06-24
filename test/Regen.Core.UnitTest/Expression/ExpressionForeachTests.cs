using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Compiler.Expressions;

namespace Regen.Core.Tests.Expression {
    [TestClass]
    public class ExpressionForeachTests : ExpressionUnitTest {
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
                    \%#1 = #2
                ";

            Compile(@input).Output
                .Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "")
                .Should()
                .BeEquivalentTo(output.Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace("\"", ""));
        }

        [TestMethod, Ignore("This is for developing purposes.")]
        public void dev_test() {
            var @input = @"
                    0
                %foreach [1.0,2.0,3.0], [2.0,3.0,4.0]%
                    -#(#1 / 3)
                %
                ";

            var c = Compile(@input);
            return;
            @input = @"[(1+2)/3f + 0, 5, [1]]";
            var comp = new RegenCompiler();
            var data = comp.EvaluateExpression((Compiler.Expressions.Expression) @input);
        }
    }
}