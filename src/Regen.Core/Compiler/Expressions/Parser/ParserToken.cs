namespace Regen.Compiler.Expressions {
    /// <summary>
    ///     Parser tokens are the higher level tokens (combinations of: ) <see cref="ExpressionToken"/>.<br></br>
    ///     They are the template parsing tokens that are on the parser level.
    /// </summary>
    public enum ParserToken {
        /// <summary>
        ///     %(expr)
        /// </summary>
        Expression,
        /// <summary>
        ///     %a = 123
        /// </summary>
        Declaration,
        /// <summary>
        ///    %foreach expr%
        ///    output#1text
        ///    output#1text-line2
        ///    %
        /// </summary>
        ForeachLoop,
        /// <summary>
        ///     %foreach expr
        ///     output#1text
        /// </summary>
        ForeachSingle,

    }
}