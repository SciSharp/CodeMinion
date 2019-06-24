using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Compiler;
using Regen.Compiler.Expressions;
using Regen.Core.Tests.Expression;
using Regen.DataTypes;
using Regen.Exceptions;
using Array = Regen.DataTypes.Array;

namespace Regen.Core.Tests {
    [TestClass]
    public class VariableTests : ExpressionUnitTest {
        [TestMethod]
        public void declare_variable_withnumber() {
            var @input = @"
                %a1 = 1
                ";

            Variables(input)
                .Keys.Should()
                .Contain("a1");
        }

        [TestMethod]
        public void declare_variable_from_other_variable() {
            var @input = @"
                %a = 1
                %a2 = a
                ";

            var variables = Variables(input);
            variables.Keys.Should()
                .ContainInOrder("a", "a2");

            variables["a"].Should().BeOfType<NumberScalar>().Which.Value.Should().Be(1);
            variables["a2"].Should().BeOfType<NumberScalar>().Which.Value.Should().Be(1);
        }

        [TestMethod]
        public void declare_variable_with_number_2() {
            var @input = @"
                %asdasdasd1 = 1
                %asdasdasd2 = [1,2,,,,,]
                ";

            Variables(input)
                .Keys.Should()
                .Contain("asdasdasd1").And
                .Contain("asdasdasd2");
        }

        [TestMethod]
        public void declare_variable_with_underscore() {
            var @input = @"
                %_asd1 = 1
                %_1asd1 = 1
                ";

            Variables(input)
                .Keys.Should()
                .Contain("_asd1").And
                .Contain("_1asd1");
        }

        [DataTestMethod]
        [DataRow("%0_aa = 1")]
        [DataRow("%1aaaaaaaaa = 1")]
        [DataRow("%aaaaaa!aaa = 1")]
        [DataRow("%aaaaaaaaa! = 1")]
        [DataRow("%a#sd2 = 1")]
        [DataRow("%1asdasdasd2# = 1")]
        [DataRow("%asdasdasd2% = 1")]
        [DataRow("%asdasd(asd2% = 1")]
        [DataRow("%!asdasd(asd2 = 1")]
        [DataRow("%asdasd]asd2 = 1")]
        [DataRow("%asdasd[asd2 = 1")]
        [DataRow("%asdasd*asd2 = 1")]
        [DataRow("%/asdasdasd2 = 1")]
        [DataRow("%#asdasdasd2 = 1")]
        [DataRow("%!asdasdasd2 = 1")]
        public void declare_variable_badnames(string input) {
            try {
                Variables(input).Keys.Should().NotContain(input);
            } catch (ExpressionException) {
                //swallowed exception
            } catch (UnexpectedTokenException) { }
        }

        [TestMethod]
        public void declare_variable_named_builtin() {
            var @input = @"
                %i = 1
                ";
            new Action(() => Variables(input)).Should()
                .Throw<ExpressionCompileException>();
        }

        [TestMethod, ExpectedException(typeof(ExpressionCompileException))]
        public void declare_variable_named_after_function_from_builtin_namespace() {
            var @input = @"
                %asarray = 1
                %(asarray(asarray)[0])
                ";
            Compile(input).Output.Should().Contain("1");
        }

        [DataTestMethod]
        [DataRow(typeof(int), "1", 1)]
        [DataRow(typeof(double), "1.0d", 1.0d)]
        [DataRow(typeof(double), "1.0D", 1.0d)]
        [DataRow(typeof(float), "1.0f", 1.0f)]
        [DataRow(typeof(float), "1.0F", 1.0f)]
        [DataRow(typeof(decimal), "1.0m", 1.0f)]
        [DataRow(typeof(decimal), "1.0M", 1.0f)]
        [DataRow(typeof(long), "1L", 1L)]
        public void declare_variable_specific_type(Type type, string emit, object value) {
            var @input = $@"
                %a = {emit}
                ";
            var variable = Variables(input).Values.First()
                .Should().BeOfType<NumberScalar>().Which;
            variable.Value.Should().BeOfType(type).And.BeEquivalentTo(value);
        }

        [TestMethod]
        public void declare_variable_specific_type_decimal() {
            var @input = $@"
                %a = 1.0
                ";
            var variable = Variables(input).Values.First().Should().BeOfType<NumberScalar>().Which;
            variable.Value.Should().BeOfType(typeof(double)).And.BeEquivalentTo(1m);
        }

        [DataTestMethod]
        [DataRow(typeof(int), "1", 1)]
        [DataRow(typeof(double), "1.0", 1.0d)]
        [DataRow(typeof(double), "1.0d", 1.0d)]
        [DataRow(typeof(double), "1.0D", 1.0d)]
        [DataRow(typeof(float), "1.0f", 1.0f)]
        [DataRow(typeof(float), "1.0F", 1.0f)]
        [DataRow(typeof(long), "1L", 1L)]
        public void declare_variable_specific_type_inarray(Type type, string emit, object value) {
            var @input = $@"
                %a = [{emit},{emit}]
                ";
            var variable = Variables(input).Values.First();
            variable.Should().BeOfType(typeof(Array));
            ((Array) variable)[1].Value.Should().BeOfType(type).And.BeEquivalentTo(value);
        }

        [TestMethod]
        public void declare_variable_null() {
            var @input = $@"
                %a = null
                ";
            var variable = Variables(input).Values.First();
            variable.Should().BeOfType(typeof(NullScalar));
        }
    }
}