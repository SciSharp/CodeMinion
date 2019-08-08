using System;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regen.Compiler.Expressions;
using Regen.Helpers;
using Regen.Parser;
using Regen.Parser.Expressions;

namespace Regen.Core.Tests.Expression {
    [TestClass]
    public class ParsingTests : ExpressionUnitTest {
        [TestMethod]
        public void input_without_template() {
            var input = @"
                
                nothing 
    of the most
of nothing
";
            var ret = Parse(input);
            ret.ETokens.Should().OnlyContain(e => e.Token == ExpressionToken.NewLine || e.Token == ExpressionToken.Literal);
        }

        [TestMethod]
        public void expression_variable_stringliteral() {
            var input = @"
                %a = ""hi""
";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            varexpr.Right.Should().BeOfType<StringLiteral>().Which.Value.Should().Be("hi");
        }

        [TestMethod]
        public void expression_variable_functionCall() {
            var input = @"
                %a = gibson(""Args1"", 15)
";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var call = varexpr.Right.Should().BeOfType<CallExpression>().Which;
            call.FunctionName.As<IdentityExpression>().Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("gibson");
            call.Arguments.Arguments.Should().HaveCount(2);
            call.Arguments.Arguments.First().Should().BeOfType<StringLiteral>().Which.Value.Should().Be("Args1");
            var operator1 = call.Arguments.Arguments.Last().Should().BeOfType<NumberLiteral>().Which.Value.Should().Be("15");
        }

        [TestMethod]
        public void expression_variable_functionCall_with_paren() {
            var input = @"
                %a = gibson(""Args1"", (1))
";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var call = varexpr.Right.Should().BeOfType<CallExpression>().Which;
            call.FunctionName.As<IdentityExpression>().Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("gibson");
            call.Arguments.Arguments.Should().HaveCount(2);
            call.Arguments.Arguments.First().Should().BeOfType<StringLiteral>().Which.Value.Should().Be("Args1");
            var operator1 = call.Arguments.Arguments.Last().Should().BeOfType<GroupExpression>()
                .Which.InnerExpression.Should().BeOfType<NumberLiteral>().Which.Value.Should().Be("1");
        }

        [TestMethod]
        public void expression_variable_functionCall_with_paren_with_mathop() {
            var input = @"
                %a = gibson(""Args1"", (1+3))
";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var call = varexpr.Right.Should().BeOfType<CallExpression>().Which;
            call.FunctionName.As<IdentityExpression>().Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("gibson");
            call.Arguments.Arguments.Should().HaveCount(2);
            call.Arguments.Arguments.First().Should().BeOfType<StringLiteral>().Which.Value.Should().Be("Args1");
            var operator1 = call.Arguments.Arguments.Last().Should().BeOfType<GroupExpression>()
                .Which.InnerExpression.Should().BeOfType<OperatorExpression>().Which;
            operator1.Left.Should().BeOfType<NumberLiteral>().Which.Value.Should().Be("1");
            operator1.Right.Should().BeOfType<NumberLiteral>()
                .Which.Value.Should().Be("3");
        }

        [TestMethod]
        public void expression_variable_functionCall_with_paren_with_two_mathop() {
            var input = @"
                %a = gibson(""Args1"", (1+2+3))
";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var call = varexpr.Right.Should().BeOfType<CallExpression>().Which;
            call.FunctionName.As<IdentityExpression>().Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("gibson");
            call.Arguments.Arguments.Should().HaveCount(2);
            call.Arguments.Arguments.First().Should().BeOfType<StringLiteral>().Which.Value.Should().Be("Args1");
            var operator1 = call.Arguments.Arguments.Last().Should().BeOfType<GroupExpression>()
                .Which.InnerExpression.Should().BeOfType<OperatorExpression>().Which;
            var operator2 = operator1.Left.Should().BeOfType<OperatorExpression>().Which;
            operator2.Left.Should().BeOfType<NumberLiteral>().Which.Value.Should().Be("1");
            operator2.Right.Should().BeOfType<NumberLiteral>().Which.Value.Should().Be("2");
            operator1.Right.Should().BeOfType<NumberLiteral>().Which.Value.Should().Be("3");
        }

        [TestMethod]
        public void expression_variable_functionCall_with_paren_with_two_mathop_and_inner_parens() {
            var input = @"
                %a = gibson(""Args1"", (1+(2+3)))
";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var call = varexpr.Right.Should().BeOfType<CallExpression>().Which;
            call.FunctionName.As<IdentityExpression>().Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("gibson");
            call.Arguments.Arguments.Should().HaveCount(2);
            call.Arguments.Arguments.First().Should().BeOfType<StringLiteral>().Which.Value.Should().Be("Args1");
            var operator1 = call.Arguments.Arguments.Last()
                .Should().BeOfType<GroupExpression>().Which.InnerExpression.Should().BeOfType<OperatorExpression>().Which;
            operator1.Left.Should().BeOfType<NumberLiteral>().Which.Value.Should().Be("1");
            var operator2 = operator1.Right.Should().BeOfType<GroupExpression>().Which
                .InnerExpression.Should().BeOfType<OperatorExpression>().Which;
            operator2.Left.Should().BeOfType<NumberLiteral>().Which.Value.Should().Be("2");
            operator2.Right.Should().BeOfType<NumberLiteral>().Which.Value.Should().Be("3");
        }

        [TestMethod]
        public void expression_variable_array() {
            var input = @"
                %a = [1,2,3]
";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var arr = varexpr.Right.Should().BeOfType<ArrayExpression>().Which;
            arr.Values.Should().HaveCount(3).And.ContainInOrder(new NumberLiteral("1"), new NumberLiteral("2"), new NumberLiteral("3"));
        }

        [TestMethod]
        public void expression_variable_array_3nests() {
            var input = @"
                %a = [[[[(3)]]]]";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var arr = varexpr.Right.Should().BeOfType<ArrayExpression>().Which;
            for (int i = 0; i < 3; i++) {
                arr.Values.Should().HaveCount(1).And.ContainItemsAssignableTo<ArrayExpression>();
                arr = (ArrayExpression) arr.Values[0];
            }

            arr.Values.Should().HaveCount(1);
            arr.Values[0].Should().BeOfType<GroupExpression>().Which.InnerExpression.Should().BeOfType<NumberLiteral>().Which.Value.Should().Be("3");
        }

        [DataTestMethod]
        [DataRow("1,'2',\"3\"")]
        [DataRow("1,'2',\"3\"")]
        [DataRow("1,'2',\"3\"")]
        [DataRow("\"3\",1,\"3\"")]
        [DataRow("'2',1,\"3\"")]
        [DataRow("'2',\"3\",1")]
        [DataRow("'2','2',1")]
        [DataRow("'2',1,'2'")]
        [DataRow("\"3\"'2',1")]
        [DataRow("\"3\",1,'2'")]
        public void expression_variable_array_mixed_types(string decl) {
            var input = $@"
                %a = [{decl}]
";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var arr = varexpr.Right.Should().BeOfType<ArrayExpression>().Which;
            arr.Values.Should().HaveCount(3);
            arr.Values.Count(e => e is NumberLiteral es && es.Value == "1" || e is CharLiteral cl && cl.Value == '2' || e is StringLiteral sl && sl.Value == "3").Should().Be(3);
        }

        [TestMethod]
        public void expression_variable_array_nested() {
            var input = @"
                %a = [1,[""1"", 5], 3 ]
";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var arr = varexpr.Right.Should().BeOfType<ArrayExpression>().Which;
            arr.Values.Should().HaveCount(3).And.ContainInOrder(new NumberLiteral("1"), new NumberLiteral("3"));
            arr.Values[1].Should().BeOfType<ArrayExpression>().Which.Values.Should().ContainInOrder(new StringLiteral("1"), new NumberLiteral("5"));
        }

        [TestMethod]
        public void expression_variable_accessor() {
            var input = @"
                %a = imsomevariable[5]
";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var indexer = varexpr.Right.Should().BeOfType<IndexerCallExpression>().Which;
            indexer.Left.Should().BeOfType<IdentityExpression>().Which.Identity
                .Should().BeOfType<StringIdentity>().Which.Name.Should().Be("imsomevariable");
            indexer.Arguments.Arguments.Should().HaveCount(1).And.ContainInOrder(new NumberLiteral("5"));
        }

        [TestMethod]
        public void expression_variable_accessor_multiple() {
            var input = @"
                %a = imsomevariable[5, 6]";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var indexer = varexpr.Right.Should().BeOfType<IndexerCallExpression>().Which;
            indexer.Left.Should().BeOfType<IdentityExpression>().Which.Identity
                .Should().BeOfType<StringIdentity>().Which.Name.Should().Be("imsomevariable");
            indexer.Arguments.Arguments.Should().HaveCount(2).And.ContainInOrder(new NumberLiteral("5"), new NumberLiteral("6"));
        }

        [TestMethod]
        public void expression_variable_bool() {
            var input = @"
                %a = true";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            varexpr.Right.Should().BeOfType<BooleanLiteral>().Which.Value.Should().Be(true);
        }

        [TestMethod]
        public void expression_variable_bool_op() {
            var input = @"
                %a = true || true";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var op = varexpr.Right.Should().BeOfType<OperatorExpression>().Which;
            op.Op.Should().Be(ExpressionToken.DoubleOr);
            op.Left.Should().BeOfType<BooleanLiteral>().Which.Value.Should().Be(true);
            op.Right.Should().BeOfType<BooleanLiteral>().Which.Value.Should().Be(true);
        }

        [TestMethod]
        public void expression_variable_char() {
            var input = @"
                %a = 'c'";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            varexpr.Right.Should().BeOfType<CharLiteral>().Which.Value.Should().Be('c');
        }

        [TestMethod]
        public void expression_variable_dictionary() {
            var input = @"
                %a = [yoo: 1223, ""yoo2"": 123]";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var arr = varexpr.Right.Should().BeOfType<ArrayExpression>().Which.Values;
            arr[0].Should().BeOfType<KeyValueExpression>()
                .Which.Key.Should().BeOfType<IdentityExpression>()
                .Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("yoo");
            arr[1].Should().BeOfType<KeyValueExpression>()
                .Which.Key.Should().BeOfType<StringLiteral>().Which.Value.Should().Be("yoo2");
            arr[0].Should().BeOfType<KeyValueExpression>()
                .Which.Value.Should().BeOfType<NumberLiteral>().Which.Value.Should().Be("1223");
            arr[1].Should().BeOfType<KeyValueExpression>()
                .Which.Value.Should().BeOfType<NumberLiteral>().Which.Value.Should().Be("123");
        }

        [TestMethod]
        public void expression_variable_dictionary_with_nestedarray() {
            var input = @"
                %a = [yoo: 1223, [""yoo2"": 123]]";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var arr = varexpr.Right.Should().BeOfType<ArrayExpression>().Which.Values;
            arr[0].Should().BeOfType<KeyValueExpression>()
                .Which.Key.Should().BeOfType<IdentityExpression>()
                .Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("yoo");

            arr[0].Should().BeOfType<KeyValueExpression>()
                .Which.Value.Should().BeOfType<NumberLiteral>().Which.Value.Should().Be("1223");
            var nested = arr[1].Should().BeOfType<ArrayExpression>().Which.Values[0].Should().BeOfType<KeyValueExpression>().Which;
            nested.Value.Should().BeOfType<NumberLiteral>().Which.Value.Should().Be("123");
            nested.Should().BeOfType<KeyValueExpression>()
                .Which.Key.Should().BeOfType<StringLiteral>().Which.Value.Should().Be("yoo2");
        }

        [TestMethod]
        public void expression_variable_mixed_dictionary_with_nestedarray() {
            var input = @"
                %a = [1223, [""yoo2"": 123]]";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var arr = varexpr.Right.Should().BeOfType<ArrayExpression>().Which.Values;
            arr[0].Should().BeOfType<NumberLiteral>().Which.Value.Should().Be("1223");

            var nested = arr[1].Should().BeOfType<ArrayExpression>().Which.Values[0].Should().BeOfType<KeyValueExpression>().Which;
            nested.Value.Should().BeOfType<NumberLiteral>().Which.Value.Should().Be("123");
            nested.Should().BeOfType<KeyValueExpression>()
                .Which.Key.Should().BeOfType<StringLiteral>().Which.Value.Should().Be("yoo2");
        }

        [TestMethod]
        public void expression_variable_accessor_from_property() {
            var input = @"
                %a = name1.name2[5]
";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var iden = varexpr.Right.Should().BeOfType<IdentityExpression>().Which;
            var prop = iden.Identity.Should().BeOfType<PropertyIdentity>().Which;
            prop.Left.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("name1");
            var indexer = prop.Right.Should().BeOfType<IndexerCallExpression>().Which;
            indexer.Arguments.Arguments.Should().ContainInOrder(new NumberLiteral("5"));
            indexer.Arguments.Arguments.Should().HaveCount(1).And.ContainInOrder(new NumberLiteral("5"));
            indexer.Left.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("name2");
        }

        [TestMethod]
        public void expression_variable_accessor_from_property_then_another_property() {
            var input = @"
                %a = name1.name2[5].josh
";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var iden = varexpr.Right.Should().BeOfType<IdentityExpression>().Which;
            var prop = iden.Identity.Should().BeOfType<PropertyIdentity>().Which;
            prop.Left.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("name1");
            var prop2 = prop.Right.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<PropertyIdentity>().Which;
            var indexer = prop2.Left.Should().BeOfType<IndexerCallExpression>().Which;
            indexer.Arguments.Arguments.Should().ContainInOrder(new NumberLiteral("5"));
            indexer.Arguments.Arguments.Should().HaveCount(1).And.ContainInOrder(new NumberLiteral("5"));
            indexer.Left.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("name2");
            prop2.Right.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("josh");
        }

        [TestMethod]
        public void expression_variable_function_from_property_then_another_property() {
            var input = @"
                %a = name1.name2(5).josh
";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var iden = varexpr.Right.Should().BeOfType<IdentityExpression>().Which;
            var prop = iden.Identity.Should().BeOfType<PropertyIdentity>().Which;
            prop.Left.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("name1");
            var prop2 = prop.Right.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<PropertyIdentity>().Which;
            var indexer = prop2.Left.Should().BeOfType<CallExpression>().Which;
            indexer.Arguments.Arguments.Should().ContainInOrder(new NumberLiteral("5"));
            indexer.Arguments.Arguments.Should().HaveCount(1).And.ContainInOrder(new NumberLiteral("5"));
            indexer.FunctionName.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("name2");
            prop2.Right.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("josh");
        }

        [TestMethod]
        public void expression_variable_function_from_property_then_another_property_plus_one() {
            var input = @"
                %a = name1.name2(5).josh.istheman
";

            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var iden = varexpr.Right.Should().BeOfType<IdentityExpression>().Which;
            var prop = iden.Identity.Should().BeOfType<PropertyIdentity>().Which;
            prop.Left.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("name1");
            var prop2 = prop.Right.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<PropertyIdentity>().Which;
            var indexer = prop2.Left.Should().BeOfType<CallExpression>().Which;
            indexer.Arguments.Arguments.Should().ContainInOrder(new NumberLiteral("5"));
            indexer.Arguments.Arguments.Should().HaveCount(1).And.ContainInOrder(new NumberLiteral("5"));
            indexer.FunctionName.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("name2");
            var prop3 = prop2.Right.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<PropertyIdentity>().Which;
            prop3.Left.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("josh");
            prop3.Right.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("istheman");
        }

        [TestMethod]
        public void expression_variable_function_from_property_then_another_property_plus_one_orishetheman() {
            var input = @"
                %a = name1.name2(5).josh.istheman[""i think not""]
";

            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var iden = varexpr.Right.Should().BeOfType<IdentityExpression>().Which;
            var prop = iden.Identity.Should().BeOfType<PropertyIdentity>().Which;
            prop.Left.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("name1");
            var prop2 = prop.Right.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<PropertyIdentity>().Which;
            var indexer = prop2.Left.Should().BeOfType<CallExpression>().Which;
            indexer.Arguments.Arguments.Should().ContainInOrder(new NumberLiteral("5"));
            indexer.Arguments.Arguments.Should().HaveCount(1).And.ContainInOrder(new NumberLiteral("5"));
            indexer.FunctionName.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("name2");
            var prop3 = prop2.Right.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<PropertyIdentity>().Which;
            prop3.Left.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("josh");
            prop3.Right.Should().BeOfType<IndexerCallExpression>().Which.Left.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("istheman");
        }

        [TestMethod]
        public void expression_variable_property() {
            var input = @"
                %a = name1.name2
";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var prop = varexpr.Right.Should().BeOfType<IdentityExpression>().Which.Identity
                .Should().BeOfType<PropertyIdentity>().Which;
            prop.Left.Should().BeOfType<IdentityExpression>()
                .Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("name1");
            prop.Right.Should().BeOfType<IdentityExpression>()
                .Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("name2");
        }

        [TestMethod]
        public void expression_variable_property_call_then_property() {
            var input = @"
                %a =  name1.name2(  3  ).aftercall
";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var prop1 = varexpr.Right.Should().BeOfType<IdentityExpression>().Which.Identity
                .Should().BeOfType<PropertyIdentity>().Which;
            prop1.Left.Should().BeOfType<IdentityExpression>().Which.Identity
                .Should().BeOfType<StringIdentity>().Which.Name.Should().Be("name1");
            var indexer = prop1.Right.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<PropertyIdentity>().Which;
            var callExpression = indexer.Left.Should().BeOfType<CallExpression>().Which;
            var prop2 = callExpression.FunctionName.Should().BeOfType<IdentityExpression>().Which.Identity
                .Should().BeOfType<StringIdentity>().Which.Name.Should().Be("name2");
            indexer.Right.Should().BeOfType<IdentityExpression>().Which.Identity
                .Should().BeOfType<StringIdentity>().Which.Name.Should().Be("aftercall");
        }

        [TestMethod]
        public void expression_variable_throw_number() {
            var input = @"
                %a = throw 1;
";
            new Action(() => {
                var ret = Parse(input);
                var act = ret.ParseActions.First();
                act.Token.Should().Be(ParserToken.Declaration);
                var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
                varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
                var indexer = varexpr.Right.Should().BeOfType<ThrowExpression>().Which;
                var prop = indexer.Right.Should().BeOfType<NumberLiteral>().Which.Value.Should().Be("1");
                throw new InvalidOperationException("Can throw a literal number.");
            }).Should().Throw<Exception>();
        }

        [TestMethod]
        public void expression_variable_null() {
            var input = @"
                %a = null;
";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var indexer = varexpr.Right.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<NullIdentity>();
        }

        [TestMethod]
        public void expression_variable_throw_new_class() {
            var input = @"
                %a = throw new Classy();
";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            var indexer = varexpr.Right.Should().BeOfType<ThrowExpression>().Which;
            var constr = indexer.Right.Should().BeOfType<NewExpression>().Which.Constructor;
            constr.Should().BeOfType<CallExpression>().Which.FunctionName.Should().BeOfType<IdentityExpression>()
                .Which.Identity.Should().BeOfType<StringIdentity>()
                .Which.Name.Should().Be("Classy");
        }

        [TestMethod]
        public void expression_variable_increment_left() {
            var input = @"
                %v = ++a
";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("v");
            var op = varexpr.Right.Should().BeOfType<LeftOperatorExpression>().Which;

            op.Op.Should().Be(ExpressionToken.Increment);
            op.Right.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
        }

        [TestMethod]
        public void expression_variable_increment_right() {
            var input = @"
                %v = a++
";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("v");
            var op1 = varexpr.Right.Should().BeOfType<RightOperatorExpression>().Which;

            op1.Left.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            op1.Op.Should().Be(ExpressionToken.Increment);
        }

        [TestMethod]
        public void expression_variable_increment_right_add() {
            var input = @"
                %v = a++ + 5
";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("v");
            var op1 = varexpr.Right.Should().BeOfType<OperatorExpression>().Which;

            var op2 = op1.Left.Should().BeOfType<RightOperatorExpression>().Which;
            op2.Left.Should().BeOfType<IdentityExpression>().Which.Identity.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("a");
            op2.Op.Should().Be(ExpressionToken.Increment);

            op1.Op.Should().Be(ExpressionToken.Add);
            op1.Right.Should().BeOfType<NumberLiteral>().Which.Value.Should().Be("5");
        }

        [TestMethod]
        public void expression_variable_increment_grouped_right_add() {
            var input = @"
                %v = (a++) + 5 + a/3
                ";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Declaration);
            var varexpr = act.Related.First().Should().BeOfType<VariableDeclarationExpression>().Which;
            varexpr.Name.Should().BeOfType<StringIdentity>().Which.Name.Should().Be("v");
        }

        [TestMethod]
        public void expression_basic() {
            var input = @"
                %(123)
                ";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Expression);
            act.Related.First()
                .Should().BeOfType<NumberLiteral>().Which.Value.Should().Be("123");
        }

        [TestMethod]
        public void expression_template() {
            var input = @"
                %template "".\file.cs"" for every ([1,2,3])
                ";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Template);
            act.Related.First()
                .Should().BeOfType<TemplateExpression>().Which.Path.Should().Contain(".\\file");
        }

        [TestMethod]
        public void expression_template_twoarguments() {
            var input = @"
                %template "".\file.cs"" for every [1,2,3],[1,2,3]
                ";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Template);
            act.Related.First()
                .Should().BeOfType<TemplateExpression>().Which.Path.Should().Contain(".\\file");
            act.Related.First()
                .Should().BeOfType<TemplateExpression>().Which.Arguments.Arguments.Length.Should().Be(2);
        }

        
        [TestMethod]
        public void expression_booleanic() {
            var input = @"
                %(1 == 1|1|2)
                ";
            var ret = Parse(input);
            var act = ret.ParseActions.First();
            act.Token.Should().Be(ParserToken.Expression);
            act.Related.First()
                .Should().BeOfType<TernaryExpression>();
        }
    }
}