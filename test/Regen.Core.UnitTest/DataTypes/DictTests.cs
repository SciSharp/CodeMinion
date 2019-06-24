using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.DataTypes;
using Dictionary = Regen.DataTypes.Dictionary;

namespace Regen.Core.Tests.DataTypes {
    //[TestClass]
    //public class DictionaryTests : DigestUnitTestBase {
    //    public Dictionary GetDictionary(params object[] additionals) {
    //        var additional = string.Join(",", additionals.Select(v => Scalar.Create(v).EmitExpressive()));
    //        if (!string.IsNullOrEmpty(additional))
    //            additional = "," + additional;
    //        var @input = $@"
    //            %a = [""hey""{additional}]
    //            ";
    //        var arr = Variables(input).Values.First();
    //        ((Scalar) arr.Should().BeOfType<Dictionary>()
    //            .Which.Values[0].Should().BeOfType<StringScalar>()
    //            .Which).Value.Should().Be("hey");

    //        return (Dictionary) arr;
    //    }

    //    [TestMethod]
    //    public void Dictionary_construct() {
    //        var @input = $@"
    //            %a = [""hey""]
    //            ";
    //        var variable = Variables(input).Values.First();
    //        ((Scalar) variable.Should().BeOfType<Dictionary>()
    //            .Which.Values[0].Should().BeOfType<StringScalar>()
    //            .Which).Value.Should().Be("hey");
    //        var arr = variable.As<Dictionary>();
    //        arr[0].Value.Should().Be("hey");
    //        arr.Count.Should().Be(1);
    //        arr.Contains("hey").Should().BeTrue();
    //    }

    //    [TestMethod]
    //    public void Dictionary_Count() {
    //        GetDictionary().Count.Should().Be(1);
    //    }

    //    [TestMethod]
    //    public void Dictionary_contains() {
    //        GetDictionary().Contains("hey").Should().BeTrue();
    //    }

    //    [TestMethod]
    //    public void Dictionary_indexof_str() {
    //        GetDictionary(1, null, "b").IndexOf("b").Should().Be(3);
    //    }

    //    [TestMethod]
    //    public void Dictionary_indexof_int() {
    //        GetDictionary(1, null, "b").IndexOf(1).Should().Be(1);
    //    }

    //    [TestMethod]
    //    public void Dictionary_lastindexof_int() {
    //        GetDictionary(1, null, "b", 1).LastIndexOf(1).Should().Be(4);
    //    }

    //    [TestMethod]
    //    public void Dictionary_lastindexof_str() {
    //        GetDictionary(1, null, "b", "b").LastIndexOf("b").Should().Be(4);
    //    }

    //    [TestMethod]
    //    public void Dictionary_indexof_null() {
    //        GetDictionary(1, null, "b").IndexOf(null).Should().Be(2);
    //    }

    //    [TestMethod]
    //    public void Dictionary_create_ints() {
    //        Dictionary.CreateParams(1, 2, 3)
    //            .Should()
    //            .ContainInOrder(Scalar.Create(1), Scalar.Create(2), Scalar.Create(3));
    //    }

    //    [TestMethod]
    //    public void Dictionary_create_nulls() {
    //        Dictionary.CreateParams(null, null, null)
    //            .Should()
    //            .ContainInOrder(Scalar.Create(null), Scalar.Create(null), Scalar.Create(null));
    //    }

    //    [TestMethod]
    //    public void Dictionary_createparams_singlenull() {
    //        Dictionary.CreateParams(null)
    //            .Should()
    //            .ContainInOrder(Scalar.Create(null));
    //    }

    //    [TestMethod]
    //    public void Dictionary_create_singlenull() {
    //        Dictionary.Create(null)
    //            .Should()
    //            .BeEmpty();
    //    }

    //    [DataTestMethod]
    //    [DataRow(typeof(int), "1", 1)]
    //    [DataRow(typeof(double), "1.0", 1.0d)]
    //    [DataRow(typeof(double), "1.0d", 1.0d)]
    //    [DataRow(typeof(double), "1.0D", 1.0d)]
    //    [DataRow(typeof(float), "1.0f", 1.0f)]
    //    [DataRow(typeof(float), "1.0F", 1.0f)]
    //    [DataRow(typeof(long), "1L", 1L)]
    //    public void create(Type type, string emit, object value) {
    //        var @input = $@"
    //            %a = [{emit},{emit},]
    //            ";
    //        var variable = Variables(input).Values.First();
    //        variable.Should().BeOfType(typeof(Regen.DataTypes.Dictionary));
    //        ((Dictionary) variable)[1].Value.Should().BeOfType(type).And.BeEquivalentTo(value);
    //    }

    //    [TestMethod]
    //    public void Dictionary_escaped_delimiter() {
    //        var @input = $@"
    //            %a = [""1"","""",""\,""]
    //            ";
    //        var variable = Variables(input).Values.First();
    //        variable.Should().BeOfType(typeof(Regen.DataTypes.Dictionary));
    //        ((Scalar) ((Dictionary) variable)[2].As<StringScalar>()).Value.Should().BeEquivalentTo(",");
    //    }


    //    [TestMethod]
    //    public void create_nested_Dictionary() {
    //        var @input = $@"
    //            %a = [1,asDictionary(1,2,3)]
    //            ";

    //        var variables = Variables(input).Values.Last();
    //        variables.Should().BeOfType<Dictionary>().Which[1].Should().BeEquivalentTo(Dictionary.CreateParams(1, 2, 3));
    //    }

    //    [TestMethod]
    //    public void create_Dictionary_with_nested_Dictionary_and_function_and_variableDictionary() {
    //        var @input = $@"
    //            %a = [1,[1,2,3],3]
    //            %b = [a,asDictionary(1,2,3),[1,2,3]]
    //            ";

    //        var variables = Variables(input).Values;
    //        var first = variables.First().Should().BeOfType<Dictionary>().Which;
    //        var last = Variables(input).Values.Last().Should().BeOfType<Dictionary>().Which;
    //        last[0].Should().BeOfType<Dictionary>().Which.Should().BeEquivalentTo(first.Should().BeOfType<Dictionary>().Which);
    //        last.Should().HaveCount(3);
    //        last[1].Should().BeOfType<Dictionary>();
    //        last[2].Should().BeOfType<Dictionary>();
    //        last[1].As<Dictionary>().Values.Should().BeEquivalentTo(last[2].As<Dictionary>().Values);
    //        last[0].As<Dictionary>()[1].Should().BeOfType<Dictionary>().Which.Should().BeEquivalentTo(last[2].As<Dictionary>().Values);
    //    }

    //    [TestMethod]
    //    public void create_Dictionary_with_nested_Dictionary_and_function_and_variableDictionary_andanother_Dictionary() {
    //        var @input = $@"
    //            %a = [1,[1,2,3],3]
    //            %b = [[1,2,3,],a,asDictionary(1,2,3),[1,2,3]]
    //            ";

    //        var variables = Variables(input).Values;
    //        var first = variables.First().Should().BeOfType<Dictionary>().Which;
    //        var last = Variables(input).Values.Last().Should().BeOfType<Dictionary>().Which;
    //        last[1].Should().BeOfType<Dictionary>().Which.Should().BeEquivalentTo(first.Should().BeOfType<Dictionary>().Which);
    //        last.Should().HaveCount(4);
    //        last[2].Should().BeOfType<Dictionary>();
    //        last[3].Should().BeOfType<Dictionary>();
    //        last[2].As<Dictionary>().Values.Should().BeEquivalentTo(last[2].As<Dictionary>().Values);
    //    }

    //    [TestMethod]
    //    public void declaration_new_nested_Dictionary_and_existing() {
    //        var @input = $@"
    //            %a = [1,asDictionary(1,2,3),[1,2,3]]
    //            ";

    //        var variable = Variables(input).Values.First().Should().BeOfType<Dictionary>().Which;
    //        variable[0].Value.Should().Be(1);
    //        variable.Should().HaveCount(3);
    //        variable[1].Should().BeOfType<Dictionary>();
    //        variable[2].Should().BeOfType<Dictionary>();
    //        variable[1].As<Dictionary>().Values.Should().BeEquivalentTo(variable[2].As<Dictionary>().Values);
    //    }
    //}
}