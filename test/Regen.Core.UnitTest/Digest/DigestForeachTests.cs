using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Regen.Core.Tests.Digest {
    [TestClass]
    public class DigestForeachTests : DigestUnitTestBase {
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
            Interpret(@input).Should()
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
            Interpret(@input).Should()
                .Contain("0").And
                .Contain("1").And
                .Contain("2");
        }

        [TestMethod]
        public void foreach_missing_endblock_implicit_EOF() {
            var @input = @"
                %a = [1|2|3]
                %foreach a%
                    #(i)
                ";
            Interpret(@input).Should()
                .Contain("0").And
                .Contain("1").And
                .Contain("2");
        }

        [TestMethod]
        public void foreach_using_builtin__vars__() {
            var @output = @"
                %a = [1|2|3]
                %b = ""hey""";

            var @input = $@"
                {@output}
                %foreach __vars__.keys()|__vars__.values()%
                    \%#1 = #2
                ";

            Interpret(@input)
                .Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "")
                .Should()
                .BeEquivalentTo(output.Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace("\"",""));
        }
    }
}