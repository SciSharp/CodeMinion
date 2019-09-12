using System;
using System.Collections.Generic;
using System.Linq;
using Regen.Compiler.Expressions;
using Regen.Compiler.Helpers;
using Regen.DataTypes;
using Regen.Exceptions;
using Regen.Helpers;
using Regen.Parser;
using Regen.Parser.Expressions;
using Array = Regen.DataTypes.Array;
using ExpressionCompileException = Regen.Exceptions.ExpressionCompileException;

namespace Regen.Compiler {
    public partial class RegenCompiler {
        #region Evaluation

        /// <summary>
        ///     Evaluate an expression, returning its return value.
        /// </summary>
        /// <param name="expression">The expression to compile and evaluate</param>
        /// <param name="_caller">Internal use.</param>
        /// <returns></returns>
        public Data EvaluateExpression(Expression expression, Type _caller = null) {
            var temps = new List<TemporaryVariable>();
            try {
                expression = HandleUnsupported(expression, temps, typeof(RegenCompiler));
                return Compute(expression, temps, _caller);
            } finally {
                if (temps.Count > 0)
                    temps.ForEach(t => t.Dispose());
            }
        }

        public object _evaluate(string expression, Line line = null) {
            //Core evaluation method, used inside all methods below.
            try {
                return Context.CompileDynamic(expression).Evaluate();
            } catch (Flee.PublicTypes.ExpressionCompileException e) {
                throw new Regen.Exceptions.ExpressionCompileException($"Was unable to evaluate expression: {expression}{(line != null ? ($" \tAt line ({line?.LineNumber}): {line?.Content}") : "")}", e);
            }
        }


        /// <summary>
        ///     This function handles unsupported features by <see cref="Regen.Flee"/> evaluator by creating temporary
        ///     variables or even changing the type of expression.
        /// </summary>
        /// <remarks>
        /// Because flee does not support arrays, we temporarly store any parsed array
        /// into a variable and then just pass the variable name, letting Regen just fetch it
        /// and pass it around.
        ///</remarks>
        public Expression HandleUnsupported(Expression expr, List<TemporaryVariable> temps, Type caller = null) {
            //todo after we support dictionaries, add support here
            switch (expr) {
                case NullIdentity _:
                case CharLiteral _:
                case NumberLiteral _:
                case StringLiteral _:
                case BooleanLiteral _:
                case StringIdentity _:
                case ReferenceIdentity _:
                case EmptyExpression _:
                    return expr;
                case ArgumentsExpression argumentsExpression: {
                    for (var i = 0; i < argumentsExpression.Arguments.Length; i++) {
                        argumentsExpression.Arguments[i] = HandleUnsupported(argumentsExpression.Arguments[i], temps, typeof(ArgumentsExpression));
                    }

                    return argumentsExpression;
                }

                case ArrayExpression arrayExpression: {
                    for (var i = 0; i < arrayExpression.Values.Length; i++) {
                        arrayExpression.Values[i] = HandleUnsupported(arrayExpression.Values[i], temps, typeof(ArrayExpression));
                    }

                    Data parsedArray = Compute(arrayExpression, temps, typeof(ArrayExpression));
                    if (parsedArray is ReferenceData rd) {
                        return IdentityExpression.WrapVariable((string) rd.Value);
                    } else {
                        var temp = new TemporaryVariable(Context, parsedArray).MarkPermanent();
                        return IdentityExpression.WrapVariable(temp.Name);
                    }

                    //todo this might lead to memory leaks!
                    //temps.Add(temp);
                    //if (caller == typeof(RegenCompiler)) { //if this is the first expression that is being parsed
                    //    temp.MarkPermanent(); 
                    //}
                }

                case IndexerCallExpression indexerCallExpression: {
                    indexerCallExpression.Left = HandleUnsupported(indexerCallExpression.Left, temps, typeof(IndexerCallExpression));
                    indexerCallExpression.Arguments = (ArgumentsExpression) HandleUnsupported(indexerCallExpression.Arguments, temps, typeof(IndexerCallExpression));
                    return indexerCallExpression;
                }

                case CallExpression callExpression: {
                    callExpression.FunctionName = HandleUnsupported(callExpression.FunctionName, temps, typeof(CallExpression));
                    callExpression.Arguments = (ArgumentsExpression) HandleUnsupported(callExpression.Arguments, temps, typeof(CallExpression));
                    return callExpression;
                }

                case IdentityExpression identityExpression: {
                    //here we turn any string literal into a reference to a variable.
                    //if theres no such variable, we assume it is for a functionname of property.
                    if (identityExpression.Identity is StringIdentity sr) {
                        if (Context.Variables.ContainsKey(sr.Name)) {
                            return new IdentityExpression(ReferenceIdentity.Wrap(sr));
                        }
                    }

                    identityExpression.Identity = HandleUnsupported(identityExpression.Identity, temps, caller ?? typeof(IdentityExpression));
                    return identityExpression;
                }

                case HashtagReferenceExpression hashtagReference: {
                    var key = $"__{hashtagReference.Number}__";
                    return new IdentityExpression(new ReferenceIdentity(key, new RegexResult() {Value = key, Index = hashtagReference.Matches().First().Index, Length = 1 + hashtagReference.Number.Length}));
                }

                case GroupExpression groupExpression:
                    groupExpression.InnerExpression = HandleUnsupported(groupExpression.InnerExpression, temps, caller ?? typeof(GroupExpression));
                    return groupExpression;
                case PropertyIdentity propertyIdentity:
                    //todo maybe here we parse Left, store and push? but first invalidate that it is not just a name.
                    propertyIdentity.Left = HandleUnsupported(propertyIdentity.Left, temps, caller ?? typeof(PropertyIdentity));
                    propertyIdentity.Right = HandleUnsupported(propertyIdentity.Right, temps, caller ?? typeof(PropertyIdentity));
                    return propertyIdentity;
                case KeyValueExpression keyValueExpression:
                    keyValueExpression.Key = HandleUnsupported(keyValueExpression.Key, temps, typeof(KeyValueExpression));
                    keyValueExpression.Value = HandleUnsupported(keyValueExpression.Value, temps, typeof(KeyValueExpression));
                    return keyValueExpression;
                case NewExpression newExpression:
                    newExpression.Constructor = HandleUnsupported(newExpression.Constructor, temps, typeof(NewExpression));
                    return newExpression;
                case LeftOperatorExpression leftOperatorExpression: {
                    var right = HandleUnsupported(leftOperatorExpression.Right, temps, typeof(LeftOperatorExpression));
                    if (OperatorExpression.IsLogicalOperator(leftOperatorExpression.Op)) {
                        switch (leftOperatorExpression.Op) {
                            case ExpressionToken.NotBoolean:
                                return new CallExpression("op_not", right);
                            case ExpressionToken.Not:
                                return new CallExpression("op_inverse", right);

                            default:
                                break;
                        }
                    }


                    leftOperatorExpression.Right = right;
                    return leftOperatorExpression;
                }

                case OperatorExpression operatorExpression: {
                    var left = HandleUnsupported(operatorExpression.Left, temps, typeof(OperatorExpression));
                    var right = HandleUnsupported(operatorExpression.Right, temps, typeof(OperatorExpression));
                    if (OperatorExpression.IsLogicalOperator(operatorExpression.Op)) {
                        switch (operatorExpression.Op) {
                            case ExpressionToken.DoubleEqual:
                                return new CallExpression("op_equals", left, right);
                            case ExpressionToken.NotEqual:
                                return new CallExpression("op_notequals", left, right);
                            case ExpressionToken.DoubleAnd:
                            case ExpressionToken.And:
                                return new CallExpression("op_and", left, right);
                            case ExpressionToken.DoubleOr:
                            case ExpressionToken.Or:
                                return new CallExpression("op_or", left, right);
                            case ExpressionToken.BiggerOrEqualThat:
                                return new CallExpression("op_biggerequals", left, right);
                            case ExpressionToken.BiggerThan:
                                return new CallExpression("op_bigger", left, right);
                            case ExpressionToken.SmallerOrEqualThat:
                                return new CallExpression("op_smallerequals", left, right);
                            case ExpressionToken.SmallerThan:
                                return new CallExpression("op_smaller", left, right);
                            case ExpressionToken.ApproxEqual:
                                return new CallExpression("op_equalsapprox", left, right);
                            default:
                                break;
                        }
                    }

                    operatorExpression.Left = left;
                    operatorExpression.Right = right;
                    return operatorExpression;
                }

                case RightOperatorExpression rightOperatorExpression:
                    rightOperatorExpression.Left = HandleUnsupported(rightOperatorExpression.Left, temps, typeof(RightOperatorExpression));
                    return rightOperatorExpression;
                case ThrowExpression throwExpression:
                    throwExpression.Right = HandleUnsupported(throwExpression.Right, temps, typeof(ThrowExpression));
                    return throwExpression;
                case TernaryExpression ternary: {
                    ternary.Condition = HandleUnsupported(ternary.Condition, temps, typeof(TernaryExpression));
                    ternary.IfTrue = HandleUnsupported(ternary.IfTrue, temps, typeof(TernaryExpression));
                    if (ternary.IfFalse != null)
                        ternary.IfFalse = HandleUnsupported(ternary.IfFalse, temps, typeof(TernaryExpression));
                    return ternary;
                }

                case ForeachExpression foreachExpression:
                case ImportExpression importExpression:
                case InteractableExpression interactableExpression:
                case VariableDeclarationExpression variableExpression:
                    throw new NotSupportedException(); //todo support? this should be found in an expression. it is a higher level expression
                case Identity identity: //this is an abstract class.
                    throw new NotSupportedException();
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     Perform the evaluation or translate <see cref="Expression"/> into a variable for later use in computation.
        /// </summary>
        /// <returns></returns>
        private Data Compute(Expression expression, List<TemporaryVariable> temps, Type _caller = null) {
            return _eval(expression, _caller);

            Data _eval(Expression express, Type caller = null) {
                var ret = innerEval();
                if (ret is Array retArray && _caller != typeof(IndexerCallExpression) && caller != typeof(CallExpression)) {
                    var name = TemporaryVariable.NewUniqueName;
                    Context.Variables[name] = retArray;
                    ret = new ReferenceData(name);
                }

                return ret;

                Data innerEval() {
                    switch (express) {
                        case null:
                            throw new NullReferenceException();
                        case ArgumentsExpression argumentsExpression: {
                            var arr = new Array();

                            foreach (var expr in argumentsExpression.Arguments) {
                                arr.Add(_eval(expr));
                            }

                            return arr;
                        }

                        case ArrayExpression arrayExpression: {
                            var arr = new Array();
                            foreach (var expr in arrayExpression.Values) {
                                arr.Add(_eval(expr));
                            }

                            return arr;
                        }

                        case NumberLiteral numberLiteral: {
                            return new NumberScalar(_evaluate(numberLiteral.Value));
                        }

                        case BooleanLiteral booleanLiteral: {
                            return new BoolScalar(booleanLiteral.Value);
                        }

                        case CharLiteral charLiteral: {
                            return new StringScalar(charLiteral.Value.ToString());
                        }

                        case NullIdentity nullIdentity: {
                            return Data.Null;
                        }

                        case StringLiteral stringLiteral: {
                            return new StringScalar(stringLiteral.Value);
                        }

                        case KeyValueExpression keyValueExpression: {
                            //todo create a KeyValue regen data type.
                            return new NetObject(new KeyValuePair<object, object>(_eval(keyValueExpression.Key), _eval(keyValueExpression.Value)));
                        }

                        case EmptyExpression emptyExpression: {
                            return Data.Null;
                        }

                        case ForeachExpression foreachExpression: {
                            break;
                        }

                        case GroupExpression groupExpression: {
                            return _eval(groupExpression.InnerExpression);
                        }

                        case ReferenceIdentity referenceIdentity: {
                            if (!Context.Variables.ContainsKey(referenceIdentity.Name))
                                return new ReferenceData(referenceIdentity.Name);

                            if (!Context.Variables.TryGetValue(referenceIdentity.Name, out var value, true))
                                throw new Exception("This should never occur.");

                            //if it is not a reference, make one.
                            if (!(value is ReferenceData)) {
                                return new ReferenceData(referenceIdentity.Name);
                            }

                            return (ReferenceData) value;
                        }

                        case PropertyIdentity propertyIdentity: {
                            TemporaryVariable tmp = null;
                            var left = _eval(propertyIdentity.Left);

                            if (left is ReferenceData rf) {
                                var right = new PropertyIdentity(IdentityExpression.WrapVariable(rf.EmitExpressive()), propertyIdentity.Right).AsString();

                                return Data.Create(_evaluate(right));
                            }


                            //not reference
                            using (tmp = new TemporaryVariable(Context, left is NetObject no ? no.Value : left)) {
                                return Data.Create(_evaluate($"{tmp.Name}.{propertyIdentity.Right.AsString()}"));
                            }

                            using (var var = new TemporaryVariable(Context, left is NetObject no ? no.Value : left)) {
                                var right = new PropertyIdentity(IdentityExpression.WrapVariable(var.Name), propertyIdentity.Right).AsString();

                                return Data.Create(_evaluate(right));
                            }
                        }

                        case StringIdentity stringIdentity: {
                            return new ReferenceData(stringIdentity.Name);
                        }

                        case IdentityExpression identityExpression: {
                            return _eval(identityExpression.Identity, caller); //todo test
                        }

                        case Identity identity: {
                            throw new NotSupportedException();
                        }

                        case CallExpression callExpression: {
                            var left = _eval(callExpression.FunctionName, typeof(CallExpression));
                            var _args = _eval(callExpression.Arguments, typeof(CallExpression));
                            Array args = _args is ReferenceData rd ? (Array) rd.UnpackReference(Context) : (Array) _args;

                            if (left is NetObject || left is Array || left is Dictionary) goto _storing;
                            //try regular parsing:

                            try {
                                var parsed = $"{left.Emit()}({args.Select(arg => arg.EmitExpressive()).StringJoin(", ")})";
                                return Data.Create(_evaluate(parsed));
                            } catch (ExpressionCompileException e) when (e.InnerException?.Message.Contains("FunctionCallElement: Could find not function") ?? false) {
                                throw;
                            } catch (ExpressionCompileException) { }

                            _storing: //try storing left as variable
                            using (var var = new TemporaryVariable(Context, left is NetObject no ? no.Value : left)) {
                                var parsed = $"{var.Name}({args.Select(arg => arg.EmitExpressive()).StringJoin(", ")})";
                                return Data.Create(_evaluate(parsed));
                            }
                        }

                        case IndexerCallExpression indexerCallExpression: {
                            var left = Data.Create(_eval(indexerCallExpression.Left, typeof(IndexerCallExpression)));
                            var _args = _eval(indexerCallExpression.Arguments, typeof(IndexerCallExpression));
                            Array args = _args is ReferenceData rd ? (Array) rd.UnpackReference(Context) : (Array) _args;

                            if (left is NetObject || left is Array || left is Dictionary) goto _storing;
                            //try regular parsing:
                            try {
                                var parsed = $"{left.Emit()}[{args.Select(arg => arg.EmitExpressive()).StringJoin(", ")}]";
                                return Data.Create(_evaluate(parsed));
                            } catch (ExpressionCompileException) { }

                            _storing: //try storing left as variable
                            using (var var = new TemporaryVariable(Context, left is NetObject no ? no.Value : left)) {
                                var parsed = $"{var.Name}[{args.Select(arg => arg.EmitExpressive()).StringJoin(", ")}]";
                                return Data.Create(_evaluate(parsed));
                            }
                        }

                        case NewExpression newExpression: {
                            //todo new
                            break;
                        }

                        case LeftOperatorExpression leftOperatorExpression: {
                            foreach (var e in leftOperatorExpression.Iterate()) {
                                if (e is ArrayExpression) throw new NotSupportedException("Unable to compile a nested array, please define it in a variable first.");
                            }

                            //todo BuiltinTests.len fails here because we do not expand left or right. we should.
                            return Data.Create(_evaluate(leftOperatorExpression.AsString()));
                        }

                        case OperatorExpression operatorExpression: {
                            foreach (var e in operatorExpression.Iterate()) {
                                if (e is ArrayExpression) throw new NotSupportedException("Unable to compile a nested array, please define it in a variable first.");
                            }

                            //todo BuiltinTests.len fails here because we do not expand left or right. we should.

                            return Data.Create(_evaluate(operatorExpression.AsString()));
                        }

                        case RightOperatorExpression rightOperatorExpression: {
                            foreach (var e in rightOperatorExpression.Iterate()) {
                                if (e is ArrayExpression) throw new NotSupportedException("Unable to compile a nested array, please define it in a variable first.");
                            }

                            //todo BuiltinTests.len fails here because we do not expand left or right. we should.
                            return Data.Create(_evaluate(rightOperatorExpression.AsString()));
                        }

                        case TernaryExpression ternary: {
                            var cond = _eval(ternary.Condition, typeof(TernaryExpression));
                            bool value = false;
                            switch (cond) {
                                case BoolScalar bs:
                                    value = bs._value;
                                    break;
                                case Data d:
                                    value = Convert.ToBoolean(d.Value);
                                    break;
                                default: {
                                    // ReSharper disable once ConstantNullCoalescingCondition
                                    throw new RegenException($"Unable to interpret \"{ternary.Condition.AsString()}\"'s result which its type is {cond?.GetType().Name ?? "null"}");
                                }
                            }

                            if (value)
                                return Data.Create(_eval(ternary.IfTrue));

                            // ReSharper disable once ConvertIfStatementToReturnStatement
                            if (ternary.IfFalse != null)
                                return Data.Create(_eval(ternary.IfFalse));

                            return Data.Null;
                        }

                        case ThrowExpression throwExpression: {
                            break;
                        }

                        case VariableDeclarationExpression variableExpression: {
                            var name = Data.Create(_eval(variableExpression.Name));
                            if (name.GetType() != typeof(StringScalar)) throw new NotSupportedException("Variable names can contain only _azAZ0-9");
                            var value = Data.Create(_eval(variableExpression.Right));
                            Context.Variables[name.ToString()] = value;
                            return value;
                        }
                    }

                    return Data.Null;
                }
            }
        }

        #endregion
    }
}