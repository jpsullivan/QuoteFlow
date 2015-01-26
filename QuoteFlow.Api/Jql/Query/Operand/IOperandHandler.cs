using System.Collections.Generic;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Jql.Query.Operand
{
    /// <summary>
    /// Knows how to perform validation on and get expanded values from <seealso cref="IOperand"/>s.
    /// </summary>
    public interface IOperandHandler<T> where T : IOperand
    {
        /// <summary>
        /// Will perform operand specific validation.
        /// </summary>
        /// <param name="searcher"> the user performing the search </param>
        /// <param name="operand"> the operand to validate </param>
        /// <param name="terminalClause"> the terminal clause that contains the operand </param>
        /// <returns> a MessageSet which will contain any validation errors or warnings or will be empty if there is nothing to
        ///         report, must not be null. </returns>
        IMessageSet Validate(User searcher, T operand, ITerminalClause terminalClause);

        /// <summary>
        /// Gets the unexpanded values provided by the user on input. In the case of a function this is the output
        /// values that will later be transformed into index values.
        /// </summary>
        /// <param name="queryCreationContext"> the context of query creation </param>
        /// <param name="operand"> the operand to get values from </param>
        /// <param name="terminalClause"> the terminal clause that contains the operand </param>
        /// <returns> a List of objects that represent this Operands raw values. This must be the values specified by the user. </returns>
        IEnumerable<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, T operand, ITerminalClause terminalClause);

        /// <returns>True if the operand represents a list of values, false otherwise. </returns>
        bool IsList();

        /// <returns>True if the operand represents the absence of a value, false otherwise. </returns>
        bool IsEmpty();

        /// <returns>True if the operand represents a function, false otherwise.</returns>
        bool IsFunction();
    }
}