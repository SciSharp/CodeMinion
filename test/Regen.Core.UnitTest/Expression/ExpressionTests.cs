using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.DataTypes;
using Regen.Helpers;
using Array = Regen.DataTypes.Array;

namespace Regen.Core.Tests.Expression {
    [TestClass]
    public class ExpressionTests : ExpressionUnitTest {
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

            var variables = UnpackedVariables(input);
            variables.Values.Should().AllBeOfType<int>();
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
        public void declaration_function_array() {
            var @input = $@"
                %a = range(0,3)
                ";

            var variables = Variables(input).Values.First();
            variables.As<Array>().Values.Select(v => v.Value).Should().BeEquivalentTo(0, 1, 2);
        }

        [TestMethod]
        public void declaration_new_nested_array() {
            var @input = $@"
                %a = [1,asarray(1,2,3)]
                ";
            var vars = Variables(input);
            vars.Values.Last().Should().BeOfType<ReferenceData>().Which.UnpackReference(vars)
                .Should().BeOfType<Array>().Which[1].Should().BeEquivalentTo(Array.CreateParams(1, 2, 3));
        }

        [TestMethod]
        public void declaration_function_str() {
            var @input = $@"
                %a = str(""hey"")
                ";

            var variables = Variables(input).Values;
            variables.Should().HaveCount(1);
            variables.First().Should().BeOfType<StringScalar>().Which.Value.Should().BeEquivalentTo("hey");
        }

        [TestMethod]
        public void expression_divideByZero() {
            new Action(() => { Variables("%(1/0)"); })
                .Should().Throw<DivideByZeroException>();
        }

        [DataTestMethod]
        [DataRow("+", 2)]
        [DataRow("-", 0)]
        [DataRow("/", 1)]
        [DataRow("*", 1)]
        public void expression_add_variables(string expression, object equalsTo) {
            var @input = $@"
                %a = 1
                %b = 1
                %(a{expression}b)
                ";

            var code = Compile(input).Output;
            code.Should().Contain(equalsTo.ToString());
        }

        [DataTestMethod]
        [DataRow("+", 2)]
        [DataRow("-", 0)]
        [DataRow("/", 1)]
        [DataRow("*", 1)]
        public void expression_emit_nothing(string expression, object equalsTo) {
            var @input = $@"
                %a = 1
                %b = 1
                %c = a+b
                ";

            var code = Compile(input).Output;
            code.Trim().Should().BeEmpty();
        }

        //todo add more tests of %() expression!


        [TestMethod]
        public void expression_access_indexer() {
            var @input = $@"
                %a = [1,2,3]
                %(a[0])
                ";

            var code = Compile(input).Output;
            code.Should().Contain("1");
        }

        [DataTestMethod]
        [DataRow("+", 2)]
        [DataRow("-", 0)]
        [DataRow("/", 1)]
        [DataRow("*", 1)]
        public void expression_access_indexer_math(string expression, object equalsTo) {
            var @input = $@"
                %a = [1,2,3]
                %b = 1
                %(a[0]{expression}b)
                ";

            var code = Compile(input).Output;
            code.Trim().Should()
                .Contain(equalsTo.ToString());
        }

        [TestMethod]
        public void expression_string_accessor() {
            var @input = $@"
                %a = ""hello""
                %b = 1
                %(a[0]+b)
                ";

            var code = Compile(input).Output;
            code.Trim().Should()
                .Contain((((int) 'h') + 1).ToString());
        }

        [TestMethod]
        public void expression_string_accessor_str() {
            var @input = $@"
                %a = ""hello""
                %b = 1
                %(str(a[0])+b)
                ";

            var code = Compile(input).Output;
            code.Trim().Should()
                .Contain("h1");
        }

        [TestMethod]
        public void expression_two_same_row() {
            var @input = $@"
                %b = 1
                %(b)%(b)
                ";

            var code = Compile(input).Output;
            code.Trim().Should()
                .Contain("11");
        }        
        
        [TestMethod]
        public void expression_two_same_row2() {
            var @input = $@"
                %b = 1
                %(b) %(b)
                ";

            var code = Compile(input).Output;
            code.Trim().Should()
                .Contain("1 1");
        }

        [TestMethod]
        public void expression_two_same_row_with_functions() {
            var @input = $@"
                %a = ""hello""
                %b = 1
                %(str(a[0])+b) %(str(a[0])+b)
                ";

            var code = Compile(input).Output;
            code.Trim().Should()
                .Contain("h1");
        }

        [TestMethod]
        public void expression_properr_array_reference_handling() {
            var @input = $@"
                %a = [1,2,3]
                %b = 1
                %(a[0]+b)
                ";

            var code = Compile(input).Output;
            code.Trim().Should()
                .Contain("2");
        }


        [TestMethod]
        public void expression_tutorial_add() {
            var @input = $@"
                %(1 + 1)
                ";

            var code = Compile(input).Output;
            code.Trim().Should()
                .Contain("2");
        }

        [TestMethod]
        public void expression_tutorial_add_float() {
            var @input = $@"
                %(1+1.1f)
                ";

            var code = Compile(input).Output;
            code.Trim().Should()
                .Contain("2.1");
        }


        [TestMethod]
        public void expression_tutorial_trailing_array() {
            var @input = $@"
                %arr = [1,2,]
                ";

            var var = Variables(input).Values.First();
            var.Should().BeOfType<Array>().Which.Should().HaveCount(2).And.ContainInOrder(new NumberScalar(1), new NumberScalar(2));
        }


        [TestMethod]
        public void expression_tutorial_array_indexing() {
            var @input = $@"
                %arr = [1,2,]
                %([1,2][0] + arr[0])
                ";

            var code = Compile(input).Output;
            code.Trim().Should()
                .Contain("2");
        }


        [TestMethod]
        public void expression_tutorial_str_concat() {
            var @input = $@"
                %str = ""there""
                %(""hi"" + "" "" + str) 
                %(""hi"" + "" "" + str + str[2])
                ";

            var code = Compile(input).Output;
            code.Trim().Should()
                .Contain("hi there").And.Contain("hi theree");
        }

        [TestMethod]
        public void expression_tutorial_array_expr_foreach() {
            var @input = $@"
                %foreach [1,2,3,4]
                    #1
                ";

            var code = Compile(input).Output;
            code.Trim().Should()
                .Contain("1").And.Contain("2").And.Contain("3").And.Contain("4");
        }

        [TestMethod]
        public void expression_tutorial_array_expr_foreach2() {
            var @input = $@"
                %foreach [1,2,3,4]
                    #(i)
                ";

            var code = Compile(input).Output.Should().NotContain("i");
        }

        [TestMethod]
        public void expression_basic_output() {
            var input = @"
                %(123)
                ";
            var ret = Compile(input).Output.Should().Contain("123").And.NotContain("%(");
        }

        [TestMethod]
        public void expression_boolean() {
            var input = @"
                %a = false
                %(a)
                ";
            var ret = Compile(input).Output.Should().Contain("false");
        }
    }
}