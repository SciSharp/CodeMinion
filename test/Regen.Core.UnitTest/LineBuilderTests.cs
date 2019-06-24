using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Compiler.Helpers;

namespace Regen.Core.Tests {
    [TestClass]
    public class LineBuilderTests {
        [TestMethod]
        public void LineBuilder_Creation() {
            string @input = @"
            a
            b
            c
            %
            
            ";

            var lines = new LineBuilder(new StringSource(@input)).Lines;
            var expected = new string[] {"", "a", "b", "c", "%", "", ""};
            foreach (var line in lines.Zip(expected, (line, exp) => (line, exp))) {
                var content = line.line.Content;
                Console.WriteLine(content);
                if (line.exp == "")
                    continue;

                content.Should().Contain(line.exp);
            }
        }
    }
}