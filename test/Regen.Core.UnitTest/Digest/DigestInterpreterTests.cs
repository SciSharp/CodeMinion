﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Compiler;
using Regen.DataTypes;
using Regen.Exceptions;
using ExpressionCompileException = Regen.Exceptions.ExpressionCompileException;

namespace Regen.Core.Tests.Digest {
    [TestClass]
    public class DigestInterpreterTests : DigestUnitTestBase {
        [TestMethod]
        public void import_static_random() {
            var @input = @"
                %(random.NextInt())
                ";
            Interpret(@input).Trim('\n', '\r', ' ', '\t', '\0').All(char.IsDigit).Should().BeTrue();
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
            var intr = new Interpreter(@input, @input, new RegenModule("mymod", new MyModule()));
            intr.Interpret(@input).Output.Trim('\n', '\r', ' ', '\t', '\0').All(char.IsDigit).Should().BeTrue();
        }

        [TestMethod]
        public void import_module_via_add() {
            var @input = @"
                %(mymod.add(1.0f, 2))
                ";
            var intr = new Interpreter(@input, @input);
            intr.AddModule(new RegenModule("mymod", new MyModule()));
            intr.Interpret(@input).Output.Trim('\n', '\r', ' ', '\t', '\0').All(char.IsDigit).Should().BeTrue();
        }

        [TestMethod]
        public void import_module_remove() {
            var @input = @"
                %(mymod.add(1.0f, 2))
                ";
            var intr = new Interpreter(@input, @input);
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
            var intr = new Interpreter(@input, @input);
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
            Interpret(input)
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

            Interpret(input).Should()
                .Contain("dynamic operator % (OperatorsOverloading lhs").And
                .Contain("left % right");
        }

        [TestMethod]
        public void pass_variables() {
            var input = @"
                %b = a + 1
                %(b)
                ";
            Interpret(input, new Dictionary<string, object>() {{"a", Scalar.Create(1)}})
                .Should().Contain("2");
        }

        [TestMethod]
        public void unescape_precentage() {
            var input = @"
                \%(b)
                ";
            Interpret(input)
                .Should().Contain("%(b)");
        }


        [TestMethod]
        public void builtin_variable_context() {
            var @input = @"
                %(__interpreter__.removemodule(""random""))
                ";
            Interpret(@input).Should().Contain("True");
        }

        [TestMethod]
        public void builtin_variable_context_try_access() {
            var @input = @"
                %(__interpreter__.removemodule(""random""))
                %(random.NextInt())
                ";
            new Action(() => { Interpret(@input); })
                .Should().Throw<ExpressionCompileException>().Where(e => e.InnerException.Message.Contains("variable with the name 'random'"));
        }

        [TestMethod]
        public void builtin_vars_get() {
            var @input = @"
                %a = 5
                %(__vars__.get(""a""))
                ";

            Variables(@input).Should().ContainValue(Data.Create(5));
        }

        [TestMethod]
        public void builtin_vars_remove() {
            var @input = @"
                %a = 5
                %(__vars__.get(""a""))
                %(__vars__.remove(""a""))
                %(__vars__.get(""a""))
                
                ";
            new Action(() => Compile(@input))
                .Should().Throw<UndefinedReferenceException>();
        }

        [TestMethod]
        public void modlue_return_self() {
            var @input = @"
                %(__vars__.self())
                ";
            Interpret(@input).Should().Contain("VariableCollectionWrapper");
        }
    }
}