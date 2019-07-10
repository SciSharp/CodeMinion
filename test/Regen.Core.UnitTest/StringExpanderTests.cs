using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Compiler.Helpers;

namespace Regen.Core.Tests {
    [TestClass]
    public class StringExpanderTests {
        [TestMethod]
        public void MultipleSplitsThenReplace() {
            var exp = new StringExpander("hello there");
            exp.SplitAt(5, true);
            exp.SplitAt(1, false);
            exp.SplitAt(6, false);
            exp.Replace(3, "y");
            exp.Content.Should().Be("heythere");
        }
    }
}