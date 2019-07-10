using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Compiler;
using Regen.DataTypes;
using Regen.Exceptions;

namespace Regen.Core.Tests.Expression {
    [TestClass]
    public class ImportTests : ExpressionUnitTest {
        [TestMethod]
        public void import_static_class() {
            var @input = @"
                %import Regen.Core.Tests.Expression.ImportMe
                %(magic(1,2))
                ";
            Compile(@input).Output.Should()
                .Contain("2");
        }

        [TestMethod]
        public void import_static_class_as() {
            var @input = @"
                %import Regen.Core.Tests.Expression.ImportMe as impme
                %(impme.magic(1,2))
                ";
            Compile(@input).Output.Should()
                .Contain("2");
        }

        [DataTestMethod]
        [DataRow("magic")]
        [DataRow("Magic")]
        [DataRow("MAGIC")]
        [DataRow("magiC")]
        [DataRow("mAgiC")]
        public void imported_object_methods_are_caseinsensitive(string str) {
            var @input = $@"
                %import Regen.Core.Tests.Expression.ImportMe as impme
                %(impme.{str}(1,2))
                ";
            Compile(@input).Output.Should()
                .Contain("2");
        }

        [TestMethod]
        public void import_module_AVoidMethod() {
            var @input = @"
                %(themod.AVoidMethod())
                ";
            Compile(@input, null, new RegenModule("themod", new SomeModule())).Output.Trim('\n', '\r', ' ', '\t').Should().BeEmpty();
        }

        [TestMethod]
        public void import_module_AVoidMethod_returns_null() {
            var @input = @"
                %a = themod.AVoidMethod()
                ";
            Variables(@input, null, new RegenModule("themod", new SomeModule())).Should().ContainKey("a").And.ContainValue(Data.Null);
        }

        [TestMethod]
        public void import_module_IgnoredMethod() {
            var @input = @"
                %(themod._IgnoredMethod())
                ";
            new Action(() => Variables(@input, null, new RegenModule("themod", new SomeModule())))
                .Should().Throw<ExpressionCompileException>()
                .Where(e => e.Message.Contains("Was unable to evaluate expression") && e.InnerException.Message.Contains("Could find not"));
        }

        [TestMethod]
        public void import_module_AReturnMethod() {
            var @input = @"
                %a = themod.AReturnMethod()
                ";
            Variables(@input, null, new RegenModule("themod", new SomeModule())).Should().ContainValue(new NumberScalar(5));
        }

        [TestMethod]
        public void import_module_AReturnMethod_overload() {
            var @input = @"
                %a = themod.AReturnMethod(3)
                ";
            Variables(@input, null, new RegenModule("themod", new SomeModule())).Should().ContainValue(new NumberScalar(4));
        }

        public class SomeModule {
            public void AVoidMethod() { }

            public void _IgnoredMethod() { }

            public int AReturnMethod() {
                return 5;
            }

            public int AReturnMethod(int arg) {
                return arg + 1;
            }
        }
    }

    public static class ImportMe {
        public static int Magic(int a, int b) {
            return a * b;
        }
    }
}