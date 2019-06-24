using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Core.Tests.Expression;
using Regen.DataTypes;
using Regen.Helpers;
using Array = Regen.DataTypes.Array;

namespace Regen.Core.Tests.DataTypes {
    [TestClass]
    public class ArrayTests : ExpressionUnitTest {
        public Array GetArray(params object[] additionals) {
            var additional = string.Join(",", additionals.Select(v => Scalar.Create(v).EmitExpressive()));
            if (!string.IsNullOrEmpty(additional))
                additional = "," + additional;
            var @input = $@"
                %a = [""hey""{additional}]
                ";
            var arr = Variables(input).Values.First();
            ((Scalar) arr.Should().BeOfType<Array>()
                .Which.Values[0].Should().BeOfType<StringScalar>()
                .Which).Value.Should().Be("hey");

            return (Array) arr;
        }

        [TestMethod]
        public void array_construct() {
            var @input = $@"
                %a = [""hey""]
                ";
            var variable = Variables(input).Values.First();
            ((Scalar) variable.Should().BeOfType<Array>()
                .Which.Values[0].Should().BeOfType<StringScalar>()
                .Which).Value.Should().Be("hey");
            var arr = variable.As<Array>();
            arr[0].Value.Should().Be("hey");
            arr.Count.Should().Be(1);
            arr.Contains("hey").Should().BeTrue();
        }

        [TestMethod]
        public void array_Count() {
            GetArray().Count.Should().Be(1);
        }

        [TestMethod]
        public void array_contains() {
            GetArray().Contains("hey").Should().BeTrue();
        }

        [TestMethod]
        public void array_indexof_str() {
            GetArray(1, null, "b").IndexOf("b").Should().Be(3);
        }

        [TestMethod]
        public void array_indexof_int() {
            GetArray(1, null, "b").IndexOf(1).Should().Be(1);
        }

        [TestMethod]
        public void array_lastindexof_int() {
            GetArray(1, null, "b", 1).LastIndexOf(1).Should().Be(4);
        }

        [TestMethod]
        public void array_lastindexof_str() {
            GetArray(1, null, "b", "b").LastIndexOf("b").Should().Be(4);
        }

        [TestMethod]
        public void array_indexof_null() {
            GetArray(1, null, "b").IndexOf(null).Should().Be(2);
        }

        [TestMethod]
        public void array_create_ints() {
            Array.CreateParams(1, 2, 3)
                .Should()
                .ContainInOrder(Scalar.Create(1), Scalar.Create(2), Scalar.Create(3));
        }

        [TestMethod]
        public void array_create_nulls() {
            Array.CreateParams(null, null, null)
                .Should()
                .ContainInOrder(Scalar.Create(null), Scalar.Create(null), Scalar.Create(null));
        }

        [TestMethod]
        public void array_createparams_singlenull() {
            Array.CreateParams(null)
                .Should()
                .ContainInOrder(Scalar.Create(null));
        }

        [TestMethod]
        public void array_create_singlenull() {
            Array.Create(null)
                .Should()
                .BeEmpty();
        }

        [DataTestMethod]
        [DataRow(typeof(int), "1", 1)]
        [DataRow(typeof(double), "1.0", 1.0d)]
        [DataRow(typeof(double), "1.0d", 1.0d)]
        [DataRow(typeof(double), "1.0D", 1.0d)]
        [DataRow(typeof(float), "1.0f", 1.0f)]
        [DataRow(typeof(float), "1.0F", 1.0f)]
        [DataRow(typeof(long), "1L", 1L)]
        public void create(Type type, string emit, object value) {
            var @input = $@"
                %a = [{emit},{emit},]
                ";
            var variable = Variables(input).Values.First();
            variable.Should().BeOfType(typeof(Regen.DataTypes.Array));
            ((Array) variable)[1].Value.Should().BeOfType(type).And.BeEquivalentTo(value);
        }

        [TestMethod]
        public void array_escaped_delimiter() {
            var @input = $@"
                %a = [""1"","""","",""]
                ";
            var variable = Variables(input).Values.First();
            variable.Should().BeOfType(typeof(Regen.DataTypes.Array));
            ((Scalar) ((Array) variable)[2].As<StringScalar>()).Value.Should().BeEquivalentTo(",");
        }

        [TestMethod]
        public void array_trailing_comma() {
            var @input = $@"
                %a = [""1"",""2"",""3"",]
                ";
            var variable = Variables(input).Values.First();
            var arr = variable.Should().BeOfType<Array>().Which;
            arr.Should().HaveCount(3);
        }


        [TestMethod, Ignore("This needs rewriting")]
        public void create_nested_array() {
            var @input = $@"
                %a = [1,asarray(1,2,3)]
                ";

            var variables = Variables(input).Values.Last();
            variables.Should().BeOfType<Array>().Which[1].Should().BeEquivalentTo(Array.CreateParams(1, 2, 3));
        }

        [TestMethod]
        public void create_array_with_nested_array_and_function_and_variablearray() {
            var @input = $@"
                %a = [1,[1,2,3],3]
                %b = [a,asarray(1,2,3),[1,2,3]]
                ";

            var variables = Variables(input);
            var values = variables.Values;
            var a = variables["a"].As<Data>().UnpackReference(variables).Should().BeOfType<Array>().Which;
            var b = variables["b"].As<Data>().UnpackReference(variables).Should().BeOfType<Array>().Which;

            b[0].UnpackReference(variables).Should().BeEquivalentTo(a);

            b.Should().HaveCount(3);
            b[variables, 1].Should().BeOfType<Array>();
            b[variables, 2].Should().BeOfType<Array>();
            b[variables, 1].As<Array>().Values.Should().BeEquivalentTo(b[2].UnpackReference(variables).As<Array>().Values);
            b[variables, 0].As<Array>()[1].UnpackReference(variables).Should().BeOfType<Array>().Which.Should().BeEquivalentTo(b[variables, 2].As<Array>().Values);
        }

        [TestMethod, Ignore("This needs rewriting")]
        public void create_array_with_nested_array_and_function_and_variablearray_andanother_array() {
            var @input = $@"
                %a = [1,[1,2,3],3]
                %b = [[1,2,3,],a,asarray(1,2,3),[1,2,3]]
                ";

            var variables = Variables(input);
            var values = variables.Values;
            var a = variables["a"].As<Data>().UnpackReference(variables).Should().BeOfType<Array>().Which;
            var b = variables["b"].As<Data>().UnpackReference(variables).Should().BeOfType<Array>().Which;

            b[0].UnpackReference(variables).Should().BeEquivalentTo(a);

            b.Should().HaveCount(3);
            b[1].UnpackReference(variables).Should().BeOfType<Array>();
            b[2].UnpackReference(variables).Should().BeOfType<Array>();
            b[1].UnpackReference(variables).As<Array>().Values.Should().BeEquivalentTo(b[2].UnpackReference(variables).As<Array>().Values);
            b[0].UnpackReference(variables).As<Array>()[1].UnpackReference(variables).Should().BeOfType<Array>().Which.Should().BeEquivalentTo(b[2].UnpackReference(variables).As<Array>().Values);

            variables = Variables(input);
            var first = variables.First().Should().BeOfType<Array>().Which;
            var last = Variables(input).Values.Last().Should().BeOfType<Array>().Which;
            last[1].Should().BeOfType<Array>().Which.Should().BeEquivalentTo(first.Should().BeOfType<Array>().Which);
            last.Should().HaveCount(4);
            last[2].Should().BeOfType<Array>();
            last[3].Should().BeOfType<Array>();
            last[2].As<Array>().Values.Should().BeEquivalentTo(last[2].As<Array>().Values);
        }

        [TestMethod, Ignore("This needs rewriting")]
        public void declaration_new_nested_array_and_existing() {
            var @input = $@"
                %a = [1,asarray(1,2,3),[1,2,3]]
                ";

            var variable = Variables(input).Values.First().Should().BeOfType<Array>().Which;
            variable[0].Value.Should().Be(1);
            variable.Should().HaveCount(3);
            variable[1].Should().BeOfType<Array>();
            variable[2].Should().BeOfType<Array>();
            variable[1].As<Array>().Values.Should().BeEquivalentTo(variable[2].As<Array>().Values);
        }
    }
}