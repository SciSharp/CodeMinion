namespace Regen.Parser {
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
        ///     %foreach expr%       <br></br>
        ///     output#1text         <br></br>
        ///     output#1text-line2   <br></br>
        ///     %                    <br></br>
        ///     or //////////////    <br></br>
        ///     %foreach expr        <br></br>
        ///     output#1text
        /// </summary>
        ForeachLoop,

        /// <summary>
        ///     %import namespace.type as aliasname
        /// </summary>
        Import,

        /// <summary>
        ///     Represents a file template atwhich this current file should be copied<br></br>
        ///     %template "path.$1.cs" for every (expr)
        /// </summary>
        Template,


    }
}