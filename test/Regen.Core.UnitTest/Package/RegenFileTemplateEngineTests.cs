using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Engine;

namespace Regen.Core.Tests.Package {
    [TestClass]
    public class RegenFileTemplateEngineTests {
        [TestMethod]
        public void parse_into_two_files() {
            var template = ResourceLoader.Current.GetEmbeddedResourceString(Assembly.GetExecutingAssembly(), "tempfilename.template.cs");
            var parsed = RegenFileTemplateEngine.Compile(template);
            parsed.Count.Should().Be(2);
            parsed[0].Path.Should().Contain("INT");
            parsed[0].Content.Should().StartWith("//Generated").And.NotContainAll("__1__", "__2__");

            parsed[1].Path.Should().Contain("FLOAT");
            parsed[1].Content.Should().StartWith("//Generated").And.NotContain("__1__", "__2__");
        }
    }
}