using System;
using System.Linq;
using System.Reflection.Emit;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Compiler;
using Regen.DataTypes;
using Regen.Exceptions;
using Array = Regen.DataTypes.Array;

namespace Regen.Core.Tests {
    [TestClass]
    public class ExpressionTests : UnitTestBase {
        [DataTestMethod]
        [DataRow("1+1", 2)]
        [DataRow("1 + 1", 2)]
        [DataRow("1 / 1", 1)]
        [DataRow("1 * 1", 1)]
        public void declaration_add_numbers(string expression, object equalsTo) {
            var @input = $@"
                %a1 = {expression}
                ";

            var variables = Variables(input);
            variables.Keys.First().Should()
                .Be("a1");
            variables.Values.First().As<NumberScalar>().Value.Should()
                .BeEquivalentTo(equalsTo);
        }        
        
        [DataTestMethod]
        [DataRow("+", 2)]
        [DataRow("-", 0)]
        [DataRow("/", 1)]
        [DataRow("*", 1)]
        public void declaration_add_variables(string expression, object equalsTo) {
            var @input = $@"
                %a = 1
                %b = 1
                %c = a{expression}b
                ";

            var variables = UnpackedVariables(input, typeof(NumberScalar));
            variables.Keys.Last().Should()
                .Be("c");
            variables.Values.Last().Should()
                .BeEquivalentTo(equalsTo);
        }

        [TestMethod]
        public void declaration_divideByZero() {
            new Action(() => { Variables("%a = 1/0"); })
                .Should().Throw<DivideByZeroException>();
        }

        [TestMethod]
        public void expression_divideByZero() {
            new Action(() => { Variables("%(1/0)"); })
                .Should().Throw<DivideByZeroException>();
        }

        //todo add more tests of %() expression!
    }
}