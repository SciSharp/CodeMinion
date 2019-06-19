using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Compiler;
using Regen.DataTypes;
using Regen.Exceptions;

namespace Regen.Core.Tests {
    [TestClass]
    public class InterpreterTests : UnitTestBase {
        [TestMethod]
        public void import_static_random() {
            var @input = @"
                %(random.NextInt())
                ";
            Interpert(@input).Trim('\n', '\r', ' ', '\t', '\0').All(char.IsDigit).Should().BeTrue();
        }

        [TestMethod]
        public void import_static_random_set_seed() {
            var @input = @"
                %(random.Seed(42))
                %a = random.NextInt()
                ";
            var a = Variables(@input).Values.First().Should().BeOfType<NumberScalar>().Which;
            var b = Variables(@input).Values.First().Should().BeOfType<NumberScalar>().Which;
            a.Should().BeEquivalentTo(b);
        }

        [TestMethod]
        public void import_module_via_construction() {
            var @input = @"
                %(mymod.add(1.0f, 2))
                ";
            var intr = new Interperter(@input, @input, new RegenModule("mymod", new MyModule()));
            intr.Interpret(@input).Output.Trim('\n', '\r', ' ', '\t', '\0').All(char.IsDigit).Should().BeTrue();
        }

        [TestMethod]
        public void import_module_via_add() {
            var @input = @"
                %(mymod.add(1.0f, 2))
                ";
            var intr = new Interperter(@input, @input);
            intr.AddModule(new RegenModule("mymod", new MyModule()));
            intr.Interpret(@input).Output.Trim('\n', '\r', ' ', '\t', '\0').All(char.IsDigit).Should().BeTrue();
        }

        [TestMethod]
        public void import_module_remove() {
            var @input = @"
                %(mymod.add(1.0f, 2))
                ";
            var intr = new Interperter(@input, @input);
            var mod = new RegenModule("mymod", new MyModule());
            intr.AddModule(mod);
            intr.Interpret(@input).Output.Trim('\n', '\r', ' ', '\t', '\0').All(char.IsDigit).Should().BeTrue();
            intr.RemoveModule(mod);
            ;
            new Action(() => { intr.Interpret(@input); })
                .Should().Throw<ExpressionCompileException>().Where(e => e.InnerException.Message.Contains("variable with the name 'mymod'"));
        }

        [TestMethod]
        public void import_module_remove_by_name() {
            var @input = @"
                %(mymod.add(1.0f, 2))
                ";
            var intr = new Interperter(@input, @input);
            var mod = new RegenModule("mymod", new MyModule());
            intr.AddModule(mod);
            intr.Interpret(@input).Output.Trim('\n', '\r', ' ', '\t', '\0').All(char.IsDigit).Should().BeTrue();
            intr.RemoveModule("mymod");
            ;
            new Action(() => { intr.Interpret(@input); })
                .Should().Throw<ExpressionCompileException>().Where(e => e.InnerException.Message.Contains("variable with the name 'mymod'"));
        }

        public class MyModule {
            public int add(double a, double b) {
                return Convert.ToInt32(a + b);
            }
        }

        [TestMethod]
        public void clean_lone_blockmarks() {
            var input = @"
                %operators = [""+""|""-""|""*""|""/""|""%""]
                %foreach operators
                    1+1
                %
                ";
            Interpert(input)
                .Should().NotContain("%");
        }

        [TestMethod]
        public void clean_lone_blockmarks_only_when_not_alone_in_row() {
            //The regex for replacing lone % detects % operator when it is not near anything as lone.

            var input = @"
                %operators = [""+""|""-""|""*""|""/""|""%""]
                %foreach operators%
                public static dynamic operator #1 (OperatorsOverloading lhs, int rhs) {
                    dynamic left = lhs.Value;
                    dynamic right = rhs;
                    return left #1 right;
                }
                %
                ";

            Interpert(input).Should()
                .Contain("dynamic operator % (OperatorsOverloading lhs").And
                .Contain("left % right");
        }

        [TestMethod]
        public void pass_variables() {
            var input = @"
                %b = a + 1
                %(b)
                ";
            Interpert(input, new Dictionary<string, object>() {{"a", Scalar.Create(1)}})
                .Should().Contain("2");
        }
    }
}