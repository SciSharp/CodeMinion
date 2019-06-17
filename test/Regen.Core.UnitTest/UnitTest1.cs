using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Compiler;

namespace Regen.Core.Tests {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestMethod1() {
            var @input = @"

                %foreach range(3,3)%
                    Console.WriteLine(""Printed #1!"");
                %
                ";

            new Interperter(@input, @input).Run().Output.Should()
                .Contain("Printed 3").And
                .Contain("Printed 4").And
                .Contain("Printed 5");
        }

        [TestMethod]
        public void TestMethod2() {
            var @input = @"
                %foreach range(3)%
                    Console.WriteLine(""Printed #1!"");
                %
                ";
            new Interperter(@input, @input).Run().Output.Should()
                .Contain("Printed 0").And
                .Contain("Printed 1").And
                .Contain("Printed 2");
        }

        [TestMethod]
        public void TestMethod3() { }

        [TestMethod]
        public void TestMethod4() { }
    }
}