using System.Collections.Generic;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Jql.Query.Operand
{
    /// <summary>
    ///  Responsible for validating <see cref="IOperand"/>s and extracting the String values from them.
    /// </summary>
    public interface IPredicateOperandResolver
    {
        /// <param name="searcher">The <see cref="User"/> performing the lookup.</param>
        /// <param name="field">A String representing the field over which you are searching.</param>
        /// <param name="operand">The Operand containing the values used to search.</param>
        /// <returns>A List of values obtained from the operand.</returns>
        IEnumerable<QueryLiteral> GetValues(User searcher, string field, IOperand operand);

        /// <summary>
        /// Returns true if the operand represents an EMPTY operand.
        /// </summary>
        /// <param name="searcher"></param>
        /// <param name="field"></param>
        /// <param name="operand">The operand to check if it is a EMPTY operand.</param>
        /// <returns>True if the operand is an EMPTY operand, false otherwise.</returns>
        bool IsEmptyOperand(User searcher, string field, IOperand operand);

        /// <summary>
        /// Returns true if the passed operand is a function call.
        /// </summary>
        /// <param name="searcher"></param>
        /// <param name="field"></param>
        /// <param name="operand">The operand to check. Cannot be null.</param>
        /// <returns>True of the passed operand is a function operand, false otherwise.</returns>
        bool IsFunctionOperand(User searcher, string field, IOperand operand);

        /// <summary>
        /// Returns true if the passed operand returns a list of values.
        /// </summary>
        /// <param name="searcher"></param>
        /// <param name="field"></param>
        /// <param name="operand">The operand to check. Cannot be null.</param>
        /// <returns>True if the passed operand returns a list of values or false otherwise.</returns>
        bool IsListOperand(User searcher, string field, IOperand operand);
    }
}