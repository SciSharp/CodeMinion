using System;
using System.Diagnostics;
using Regen.Exceptions;

namespace Regen.Compiler.Expressions {
    public class LeftOperatorExpression : Expression, IOperatorExpression {
        public ExpressionToken Op;
        public Expression Right;

        public LeftOperatorExpression(ExpressionToken op, Expression right) {
            Op = op;
            Right = right;
        }

        public static bool IsCurrentAnLeftUniOperation(ExpressionWalker ew) {
            switch (ew.Current.Token) {
                case ExpressionToken.Increment:
                case ExpressionToken.Decrement:
                case ExpressionToken.Add:
                case ExpressionToken.Sub:
                case ExpressionToken.Not:
                case ExpressionToken.Xor:
                case ExpressionToken.NotBoolean:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsNextAnLeftUniOperation(ExpressionWalker ew) {
            if (!ew.HasNext)
                return false;

            using (ew.CheckPoint()) {
                ew.Next();
                return IsCurrentAnLeftUniOperation(ew);
            }
        }

        public static LeftOperatorExpression Parse(ExpressionWalker ew, Expression known = null) {
            if (!IsCurrentAnLeftUniOperation(ew))
                throw new InvalidTokenException(ExpressionToken.Increment, ew.Current.Token);

            var op = ew.Current.Token;
            ew.NextOrThrow();
            var right = ew.ParseExpression(typeof(LeftOperatorExpression));
            return new LeftOperatorExpression(op, right);
        }

        ExpressionToken IOperatorExpression.Op {
            get => Op;
            set => Op = value;
        }
    }

    public class RightOperatorExpression : Expression, IOperatorExpression {
        public Expression Left;
        public ExpressionToken Op;

        public RightOperatorExpression(Expression left, ExpressionToken op) {
            Left = left;
            Op = op;
        }

        public static bool IsNextAnRightUniOperation(ExpressionWalker ew, Type caller) {
            if (!ew.HasNext)
                return false;

            using (ew.CheckPoint()) {
                ew.Next();
                return IsCurrentAnRightUniOperation(ew);
            }
        }

        public static bool IsCurrentAnRightUniOperation(ExpressionWalker ew) {
            switch (ew.Current.Token) {
                case ExpressionToken.Increment:
                case ExpressionToken.Decrement:
                case ExpressionToken.NullCoalescing:
                    return true;
                default:
                    return false;
            }
        }

        public static Expression Parse(ExpressionWalker ew, Expression known = null) {
            var left = known ?? ew.ParseExpression(typeof(RightOperatorExpression));

            if (!IsCurrentAnRightUniOperation(ew))
                return left;

            var op = ew.Current.Token;
            ew.NextOrThrow();
            return new RightOperatorExpression(left, op);
        }

        ExpressionToken IOperatorExpression.Op {
            get => Op;
            set => Op = value;
        }
    }

    public interface IOperatorExpression {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ExpressionToken Op { get; set; }
    }

    public class OperatorExpression : Expression, IOperatorExpression {
        public Expression Left;
        public ExpressionToken Op;
        public Expression Right;

        public OperatorExpression(Expression lexpr, ExpressionToken op, Expression rexpr) {
            Left = lexpr;
            Op = op;
            Right = rexpr;
        }

        private OperatorExpression() { }

        public static bool IsNextAnOperation(ExpressionWalker ew) {
            if (!ew.HasNext)
                return false;

            using (ew.CheckPoint()) {
                ew.Next();
                return IsCurrentAnOperation(ew);
            }
        }

        public static bool IsCurrentAnOperation(ExpressionWalker ew) {
            switch (ew.Current.Token) {
                case ExpressionToken.Increment:
                case ExpressionToken.Decrement:
                case ExpressionToken.Add:
                case ExpressionToken.Sub:
                case ExpressionToken.Pow:
                case ExpressionToken.Mul:
                case ExpressionToken.Div:
                case ExpressionToken.DoubleEqual:
                case ExpressionToken.NotEqual:
                case ExpressionToken.Equal:
                case ExpressionToken.DoubleAnd:
                case ExpressionToken.And:
                case ExpressionToken.DoubleOr:
                case ExpressionToken.Or:
                case ExpressionToken.Not:
                case ExpressionToken.Xor:
                case ExpressionToken.ShiftRight:
                case ExpressionToken.BiggerOrEqualThat:
                case ExpressionToken.BiggerThan:
                case ExpressionToken.ShiftLeft:
                case ExpressionToken.SmallerOrEqualThat:
                case ExpressionToken.SmallerThan:
                case ExpressionToken.RangeTo:
                    return true;
                case ExpressionToken.NullCoalescing:
                    return true; //todo make a specific custom op expressonfor null coalescing.
                default:
                    return false;
            }
        }

        public static Expression Parse(ExpressionWalker ew, Expression left = null) {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var ret = new OperatorExpression();
            ret.Left = left ?? ew.ParseExpression(typeof(OperatorExpression));
            if (!IsCurrentAnOperation(ew))
                return left;
            ret.Op = ew.Current.Token;
            ew.NextOrThrow();
            ret.Right = ew.ParseExpression(typeof(OperatorExpression));
            return ret;
        }

        ExpressionToken IOperatorExpression.Op {
            get => Op;
            set => Op = value;
        }
    }
}