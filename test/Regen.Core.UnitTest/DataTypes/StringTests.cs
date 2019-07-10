using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Core.Tests.Expression;
using Regen.DataTypes;

namespace Regen.Core.Tests.DataTypes {
    [TestClass]
    public class StringTests : ExpressionUnitTest {
        [TestMethod]
        public void string_indexer() {
            var @input = $@"
                %a = ""hello""
                %b = a[0]
                ";
            var variable = Variables(input).Values.Last();
            variable.Should().BeOfType(typeof(NumberScalar));
            variable.As<NumberScalar>().Value.As<char>().Should().Be('h');
        }

        [TestMethod]
        public void string_substring() {
            var @input = $@"
                %a = ""hello""
                %b = a.Substring(0,1)
                ";
            var variable = Variables(input).Values.Last();
            variable.Should().BeOfType(typeof(StringScalar));
            ((Scalar) variable.As<StringScalar>()).Value.As<string>().Should().Be("h");
        }

        [TestMethod]
        public void string_indexer_substring() {
            var @input = $@"
                %a = ""hello""
                %b = a[0,1]
                ";
            var variable = Variables(input).Values.Last();
            variable.Should().BeOfType(typeof(StringScalar));
            ((Scalar) variable.As<StringScalar>()).Value.As<string>().Should().Be("h");
        }

        [TestMethod]
        public void string_indexer_substring_overflow() {
            var @input = $@"
                %a = ""hello""
                %b = a[6,1]
                ";
            var variable = Variables(input).Values.Last();
            variable.Should().BeOfType(typeof(StringScalar));
            ((Scalar) variable.As<StringScalar>()).Value.As<string>().Should().Be("");
        }

        [TestMethod]
        public void string_indexer_substring_overflow_count() {
            var @input = $@"
                %a = ""hello""
                %b = a[0,15]
                ";
            var variable = Variables(input).Values.Last();
            variable.Should().BeOfType(typeof(StringScalar));
            ((Scalar) variable.As<StringScalar>()).Value.As<string>().Should().Be("hello");
        }

        [TestMethod]
        public void string_indexer_overflow() {
            var @input = $@"
                %a = ""hello""
                %b = a[5]
                ";
            var variable = Variables(input).Values.Last();
            variable.Should().BeOfType(typeof(NumberScalar));
            variable.As<NumberScalar>().Value.As<char>().Should().Be('\0');
        }
    }
}