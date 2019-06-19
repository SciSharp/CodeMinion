using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Regen.Core.Tests {
    [TestClass]
    public class InterpreterTests : UnitTestBase {
        [TestMethod]
        public void clean_lone_blockmarks() {
            var input = @"
                %operators = [""+""|""-""|""*""|""/""|""%""]
                %foreach operators
                    1+1
                %
                ";
            Interpert(input)
                .Should().NotContain("%");
        }

        [TestMethod]
        public void clean_lone_blockmarks_only_when_not_alone_in_row() {
            //The regex for replacing lone % detects % operator when it is not near anything as lone.

            var input = @"
                %operators = [""+""|""-""|""*""|""/""|""%""]
                %foreach operators%
                public static dynamic operator #1 (OperatorsOverloading lhs, int rhs) {
                    dynamic left = lhs.Value;
                    dynamic right = rhs;
                    return left #1 right;
                }
                %
                ";

            Interpert(input).Should()
                .Contain("dynamic operator % (OperatorsOverloading lhs").And
                .Contain("left % right");
        }
    }
}