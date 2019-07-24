using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Regen.Core.Tests.Expression {
    [TestClass]
    public class CommentTests : ExpressionUnitTest {
        [TestMethod]
        public void comment_at_foreach() {
            var @input = @"

                %foreach range(3,3)% #//this should be gone
                    Console.WriteLine(""Printed #1!"");
                %
                ";

            Compile(input).Output
                .Should()
                .NotContain("#//this should be gone").And
                .NotContain("%");
        }

        [TestMethod]
        public void comment_after_foreach() {
            var @input = @"
                %foreach range(3,3)% 
                    Console.WriteLine(""Printed #1!"");
                % #//this should be gone
                ";

            Compile(input).Output
                .Should()
                .NotContain("#//this should be gone");
        }

        [TestMethod]
        public void comment_swallows_entire_line() {
            var @input = @"
                #//%foreach range(3,3)% 
                    Console.WriteLine(""Printed #1!"");
                #//this should be gone % #//this should be gone
                ";

            Compile(input).Output
                .Should()
                .NotContain("#//this should be gone").And
                .NotContain("foreach");
        }

        [TestMethod]
        public void comment_escaped_swallows_entire_line() {
            var @input = @"
                #// %foreach range(3,3)
                    Console.WriteLine(""Printed #1!"");
                #//this should be gone % #//this should be gone
                ";

            Compile(input).Output
                .Should()
                .NotContain("//%foreach range(3,3)")
                .And.Subject.Should().NotContainAll("Printed 3!", "Printed 4!", "Printed 5!");
        }

        [TestMethod]
        public void comment_alone() {
            var @input = @"
                #//this should be gone
                ";

            Compile(input).Output
                .Should()
                .NotContain("#//this should be gone");
        }

        [TestMethod]
        public void comment_escaped() {
            var @input = @"
                \#//the # should be gone
                ";

            Compile(input).Output
                .Should()
                .Contain("//the # should be gone").And.NotContain("\\#");
        }
    }
}