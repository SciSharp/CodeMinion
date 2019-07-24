using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Compiler;
using Regen.Core.Tests.Expression;
using Regen.DataTypes;
using Regen.Exceptions;
using Regen.Parser;
using ExpressionCompileException = Regen.Exceptions.ExpressionCompileException;

namespace Regen.Core.Tests {
    [TestClass]
    public class RegenCompilerTests : ExpressionUnitTest {
        [TestMethod]
        public void import_static_random() {
            var @input = @"
                %(random.NextInt())
                ";
            Compile(@input).Output.Trim('\n', '\r', ' ', '\t', '\0').All(char.IsDigit).Should().BeTrue();
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
            var parsed = ExpressionParser.Parse(input);
            var comp = new RegenCompiler(new RegenModule("mymod", new MyModule()));
            var output = comp.Compile(parsed);
            output.Trim('\n', '\r', ' ', '\t', '\0').All(char.IsDigit).Should().BeTrue();
        }

        [TestMethod]
        public void import_module_via_add() {
            var @input = @"
                %(mymod.add(1.0f, 2))
                ";
            var parsed = ExpressionParser.Parse(input);
            var comp = new RegenCompiler(new RegenModule("mymod", new MyModule()));
            var output = comp.Compile(parsed);
            output.Trim('\n', '\r', ' ', '\t', '\0').All(char.IsDigit).Should().BeTrue();
        }

        [TestMethod]
        public void import_module_remove() {
            var @input = @"
                %(mymod.add(1.0f, 2))
                ";
            var parsed = ExpressionParser.Parse(input);
            var comp = new RegenCompiler();
            var mod = new RegenModule("mymod", new MyModule());
            comp.AddModule(mod);
            comp.Compile(parsed).Trim('\n', '\r', ' ', '\t', '\0').All(char.IsDigit).Should().BeTrue();
            comp.RemoveModule(mod).Should().BeTrue();

            new Action(() => { comp.Compile(ExpressionParser.Parse(input)); })
                .Should().Throw<ExpressionCompileException>().Where(e => e.InnerException.Message.Contains("variable with the name 'mymod'"));
        }

        [TestMethod]
        public void import_module_remove_by_name() {
            var @input = @"
                %(mymod.add(1.0f, 2))
                ";
            var parsed = ExpressionParser.Parse(input);
            var comp = new RegenCompiler();
            var mod = new RegenModule("mymod", new MyModule());
            comp.AddModule(mod);
            comp.Compile(parsed).Trim('\n', '\r', ' ', '\t', '\0').All(char.IsDigit).Should().BeTrue();
            comp.RemoveModule("mymod");

            new Action(() => { comp.Compile(ExpressionParser.Parse(input)); })
                .Should().Throw<ExpressionCompileException>().Where(e => e.InnerException.Message.Contains("variable with the name 'mymod'"));
        }

        [TestMethod]
        public void reuse_parsedCode() {
            var @input = @"
                %(mymod.add(1.0f, 2))
                ";
            var parsed = ExpressionParser.Parse(input);
            var comp = new RegenCompiler();
            var mod = new RegenModule("mymod", new MyModule());
            comp.AddModule(mod);
            comp.Compile(parsed).Trim('\n', '\r', ' ', '\t', '\0').All(char.IsDigit).Should().BeTrue();
            comp.RemoveModule("mymod");

            new Action(() => { comp.Compile(parsed); })
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
                %operators = [""+"",""-"",""*"",""/"",""%""]
                %foreach operators
                    1+1
                %
                ";
            Compile(@input).Output
                .Should().NotContain("%");
        }

        [TestMethod]
        public void clean_lone_blockmarks_only_when_not_alone_in_row() {
            //The regex for replacing lone % detects % operator when it is not near anything as lone.

            var input = @"
                %operators = [""+"",""-"",""*"",""/"",""%""]
                %foreach operators%
                public static dynamic operator #1 (OperatorsOverloading lhs, int rhs) {
                    dynamic left = lhs.Value;
                    dynamic right = rhs;
                    return left #1 right;
                }
                %
                ";

            Compile(@input).Output.Should()
                .Contain("dynamic operator % (OperatorsOverloading lhs").And
                .Contain("left % right");
        }

        [TestMethod]
        public void pass_variables() {
            var input = @"
                %b = a + 1
                %(b)
                ";
            Compile(input, new Dictionary<string, object>() {{"a", Data.Create(1)}}).Output
                .Should().Contain("2");
        }

        [TestMethod]
        public void unescape_precentage() {
            var input = @"
                \%(b)
                ";
            Compile(@input).Output
                .Should().Contain("%(b)");
        }


        [TestMethod]
        public void builtin_variable_context() {
            var @input = @"
                %(__compiler__.removemodule(""random""))
                ";
            Compile(@input).Output.Should().Contain("true");
        }

        [TestMethod]
        public void builtin_variable_context_try_access() {
            var @input = @"
                %(__compiler__.removemodule(""random""))
                %(random.NextInt())
                ";
            new Action(() => { Compile(@input); })
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
        public void module_return_self() {
            var @input = @"
                %(__vars__.self())
                ";
            Compile(@input).Output.Should().Contain("VariableCollectionWrapper");
        }

        [TestMethod]
        public void complex_expression_1() {
            var @input = @"
                    %a = [1,2,3,4]
                    %b = [null,""gig"",""yo"",""hi"",]
                    %c = 123 - 412
                    
                    %d1 = 123.0f + 123.0d
                    %d2 = 123f
                    %d4 = 123
                    %d5 = 123.0M
                    
                    %e1 = ""hey there pretty!%@%$%#\""@%Ifdscvxcv #!@#!""
                    %e2 = len(a) + 1
                    %e3 = a[len(a) - 2]
                    %e3 = a[len(a) - 2]+3
                ";

            new Action(() => Compile(@input)).Should().NotThrow();
        }

        [TestMethod]
        public void trailing_precentage() {
            var @input = @"
                 %a = [""NDArray"",""Double"",""Single"",""Decimal"",""Int32"",""Byte"",""Int16"",""UInt16"",""UInt32"",""Int64"",""UInt64"",""Char"",""Complex"",""String"",""Boolean"",""Object""]
                %indexes = [""i"",""j"",""k"",""m"",""n"",""g"",""h"",""e"",""f""]
                switch (DType.Name) 
                {
                %foreach a%
                    case ""#1"": 123123
                        return _array#1;
                %
                ";

            Compile(@input).Output.Should().NotContain("%");
        }

        [TestMethod]
        public void foreach_hashtag_inside_hashtag_expression() {
            var @input = @"
                %supportedTypeCodes = [""Boolean"",""Byte""]
                %foreach supportedTypeCodes%
                case TypeCode.#1:
                {
                    newValues = new #(#1)[arrayVar.Length];
                    for (int idx = 0; idx < arrayVar.Length; idx++)
                        newValues.SetValue(Convert.To#1(arrayVar.GetValue(idx)), idx);
                    break;
                }
                %

                ";

            Compile(@input).Output.Should().Contain("new Boolean[");
        }

        [TestMethod]
        public void foreach_hashtag_inside_hashtag_expression_2() {
            var @input = @"
                %supportedTypeCodes = [""Boolean"",""Byte""]
                %foreach supportedTypeCodes%
                    newValues = new #(str(#1).tolower())[arrayVar.Length];
                    for (int idx = 0; idx < arrayVar.Length; idx++)
                        newValues.SetValue(Convert.To#1(arrayVar.GetValue(idx)), idx);
                    break;
                %

                ";

            Compile(@input).Output.Should().Contain("new boolean[");
        }

        [TestMethod]
        public void foreach_hashtag_inside_hashtag_expression_3()
        {
            var @input = @"
                %supportedTypeCodes = [""Boolean"",""Byte""]
                %foreach supportedTypeCodes%
                case TypeCode.#1:
                {
                    newValues = new #(#1)[arrayVar.Length];
                    for (int idx = 0; idx < arrayVar.Length; idx++)
                        newValues.SetValue(Convert.To#1(arrayVar.GetValue(idx)), idx);
                    break;
                }
                %

                ";

            Compile(@input).Output.Should().Contain("new Boolean[");
        }

        [TestMethod]
        public void foreach_hashtag_inside_hashtag_expression_two_in_a_row()
        {
            var @input = @"
                    %types = [""short"",""int"",""long""]
                    %foreach types%
                        var #(#1.substring(0,2)+"" "")= ""#(#1.toupper())""
                    %
                ";

            Compile(@input).Output.Should().Contain("var sh = \"SHORT\"");

        }
    }
}