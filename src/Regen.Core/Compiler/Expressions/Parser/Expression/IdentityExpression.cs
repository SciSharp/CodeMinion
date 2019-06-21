using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Regen.Compiler.Expressions {
    /// <summary>
    ///     Parses cases like: <br></br>
    ///     justname<br></br>
    ///     justname.accessor<br></br>
    ///     justname.accessor[5].second<br></br>
    ///     justname.method().someval<br></br>
    ///     justname.method().someval[3].thatsfar
    /// </summary>
    public class IdentityExpression : Expression {
        public Expression Identity;

        public IdentityExpression(Expression identity) {
            Identity = identity;
        }

        private IdentityExpression() { }

        public static IdentityExpression Parse(ExpressionWalker ew, Type caller = null, Expression left = null) {
            var ret = new IdentityExpression();
            //types:
            //justname
            //justname.accessor
            //justname.accessor[5].second
            //justname.method().someval
            //justname.method().someval[3].thatsfar

            if (left == null)
                ew.IsCurrentOrThrow(ExpressionToken.Literal);
            if (ew.PeakNext.WhitespacesAfterMatch == 0 && ew.PeakNext.Token == ExpressionToken.Period && caller != typeof(IdentityExpression) || left != null) {
                var first = left ?? ew.ParseExpression(typeof(IdentityExpression));
                ew.NextOrThrow(); //skip the predicted period.
                var next = ew.PeakNext;
                switch (next.Token) {
                    //currently we are at Literal
                    case ExpressionToken.Period:
                        ret.Identity = new PropertyIdentity(first, Parse(ew));
                        return ret;
                    case ExpressionToken.LeftBracet:
                        ret.Identity = new PropertyIdentity(first, IndexerCallExpression.Parse(ew));
                        return ret;
                    case ExpressionToken.LeftParen:
                        ret.Identity = new PropertyIdentity(first, CallExpression.Parse(ew));
                        return ret;
                    default:
                        if (left != null) {
                            return new IdentityExpression(new PropertyIdentity(left, Parse(ew, typeof(IdentityExpression), null)));
                        }

                        if (first != null) {
                            //just a plain single word!
                            var right = new IdentityExpression();
                            if (ew.IsCurrent(ExpressionToken.Null)) {
                                right.Identity = NullIdentity.Instance;
                            } else {
                                ew.IsCurrentOrThrow(ExpressionToken.Literal);
                                right.Identity = StringIdentity.Create(ew.Current.Match);
                            }

                            ew.Next();
                            return new IdentityExpression(new PropertyIdentity(first, right));
                        }

                        goto _plain_identity;
                }
            }

            _plain_identity:
            //just a plain single word!
            if (ew.IsCurrent(ExpressionToken.Null)) {
                ret.Identity = NullIdentity.Instance;
            } else {
                ew.IsCurrentOrThrow(ExpressionToken.Literal);
                ret.Identity = StringIdentity.Create(ew.Current.Match);
            }

            ew.Next();
            return ret;
        }

        public override IEnumerable<Match> Matches() {
            foreach (var match in Identity.Matches()) {
                yield return match;
            }
        }
    }
}