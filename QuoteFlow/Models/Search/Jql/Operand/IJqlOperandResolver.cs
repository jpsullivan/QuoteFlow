using System.Collections.Generic;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Query.Operand;

namespace QuoteFlow.Models.Search.Jql.Operand
{
    /// <summary>
    /// Responsible for validating <see cref="IOperand"/>s and extracting the
    /// <see cref="QueryLiteral"/> values from them.
    /// </summary>
    public interface IJqlOperandResolver
    {
        string Validate(User searcher, IOperand operand, IWasClause clause);

        /// <summary>
        /// Return the values contained within the passed operand.
        /// </summary>
        /// <param name="searcher"> the user performing the search</param>
        /// <param name="operand"> the operand whose values should be returned. Must not be null.</param>
        /// <param name="terminalClause"> the terminal clause that contained the operand</param>
        /// <returns>A list of the values in the literal. May return null on an error. </returns>
        IEnumerable<QueryLiteral> GetValues(User searcher, IOperand operand, ITerminalClause terminalClause);

        /// <summary>
        /// Return the values contained within the passed operand.
        /// </summary>
        /// <param name="queryCreationContext"> the context of query creation </param>
        /// <param name="operand"> the operand whose values should be returned. Must not be null. </param>
        /// <param name="terminalClause"> the terminal clause that contained the operand </param>
        /// <returns> a list of the values in the literal. May return null on an error. </returns>
        IList<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, IOperand operand, ITerminalClause terminalClause);

        /// <summary>
        /// Validates the operand against its handler.
        /// </summary>
        /// <param name="user"> the user who is validating. May be null. </param>
        /// <param name="operand"> the operand to be validated. Must not be null. </param>
        /// <param name="terminalClause"> the terminal clause that contained the operand </param>
        /// <returns> a <seealso cref="com.atlassian.jira.util.MessageSet"/> containing any errors reported. Note: if the operand is
        /// unknown, an error message will be added to the message set returned. Never null. </returns>
        IMessageSet Validate(User user, IOperand operand, ITerminalClause terminalClause);

        /// <summary>
        /// Sanitise a function operand for the specified user, so that information is not leaked.
        /// </summary>
        /// <param name="searcher"> the user performing the search </param>
        /// <param name="operand"> the operand to sanitise; will only be sanitised if valid </param>
        /// <returns> the sanitised operand; never null. </returns>
        FunctionOperand SanitiseFunctionOperand(User searcher, FunctionOperand operand);

        /// <summary>
        /// Returns the single value contained within the passed operand. If the operand contains more than one value, an
        /// exception is thrown.
        /// </summary>
        /// <param name="user"> the user who is retrieving the values. May be null. </param>
        /// <param name="operand"> the operand whose values should be returned. Must not be null. </param>
        /// <param name="clause"> the terminal clause that contained the operand </param>
        /// <returns> the single value present in the operand, or null if there is no value. </returns>
        /// <exception cref="IllegalArgumentException"> if the operand contains more than one value. </exception>
        QueryLiteral GetSingleValue(User user, IOperand operand, ITerminalClause clause);

        /// <summary>
        /// Returns true if the operand represents an EMPTY operand.
        /// </summary>
        /// <param name="operand"> the operand to check if it is a EMPTY operand </param>
        /// <returns> true if the operand is an EMPTY operand, false otherwise. </returns>
        bool IsEmptyOperand(IOperand operand);

        /// <summary>
        /// Returns true if the passed operand is a function call.
        /// </summary>
        /// <param name="operand"> the operand to check. Cannot be null. </param>
        /// <returns> true of the passed operand is a function operand, false otherwise. </returns>
        bool IsFunctionOperand(IOperand operand);

        /// <summary>
        /// Returns true if the passed operand returns a list of values.
        /// </summary>
        /// <param name="operand"> the operand to check. Cannot be null. </param>
        /// <returns> true if the passed operand returns a list of values or false otherwise. </returns>
        bool IsListOperand(IOperand operand);

        /// <summary>
        /// Returns true if the operand is one which is known about. This is:
        /// 
        /// <ul>
        /// <li><seealso cref="SingleValueOperand"/>s
        /// <li><seealso cref="MultiValueOperand"/>s
        /// <li><seealso cref="EmptyOperand"/>s
        /// <li><seealso cref="FunctionOperand"/>s registered as <seealso cref="JqlFunction"/>s
        /// </ul>
        /// </summary>
        /// <param name="operand"> the operand; cannot be null. </param>
        /// <returns> true if it is known, false otherwise. </returns>
        bool IsValidOperand(IOperand operand);
    }
}