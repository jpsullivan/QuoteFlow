namespace QuoteFlow.Api.Jql.Query.Clause
{
    public interface IClauseVisitor<T>
    {
        /// <summary>
        /// Visit called when accepting a <see cref="AndClause"/>.
        /// </summary>
        /// <param name="andClause"> the node being visited.
        /// </param>
        /// <returns>The return type specified by the visitor. </returns>
        T Visit(AndClause andClause);

        /// <summary>
        /// Visit called when accepting a <see cref="NotClause"/>.
        /// </summary>
        /// <param name="notClause"> the node being visited.
        /// </param>
        /// <returns>The return type specified by the visitor. </returns>
        T Visit(NotClause notClause);

        /// <summary>
        /// Visit called when accepting a <see cref="OrClause"/>.
        /// </summary>
        /// <param name="orClause"> the node being visited.
        /// </param>
        /// <returns>The return type specified by the visitor. </returns>
        T Visit(OrClause orClause);

        /// <summary>
        /// Visit called when accepting a <see cref="TerminalClause"/>.
        /// </summary>
        /// <param name="clause"> the node being visited.
        /// </param>
        /// <returns>The return type specified by the visitor. </returns>
        T Visit(ITerminalClause clause);


        /// <summary>
        /// Visit called when accepting a <see cref="WasClause"/>.
        /// </summary>
        /// <param name="clause"> the node being visited.
        /// </param>
        /// <returns>The return type specified by the visitor.</returns>
        T Visit(IWasClause clause);


        /// <summary>
        /// Visit called when accepting a <see cref="ChangedClause"/>.
        /// </summary>
        /// <param name="clause"> the node being visited.
        /// </param>
        /// <returns>The return type specified by the visitor.</returns>
        T Visit(IChangedClause clause);
    }
}