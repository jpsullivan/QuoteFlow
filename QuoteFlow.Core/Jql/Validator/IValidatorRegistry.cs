using System.Collections.Generic;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Jql.Validator
{
    /// <summary>
    /// Resolves the validators for a provided <see cref="TerminalClause"/>.
    /// </summary>
    public interface IValidatorRegistry
    {
        /// <summary>
        /// Fetches the associated ClauseValidators for the provided TerminalClause. The returned value is based on
        /// the clause's name and the <see cref="com.atlassian.query.operator.Operator"/> that is associated with the
        /// provided clause. Multiple values may be returned for custom fields.
        /// </summary>
        /// <param name="searcher"> the user who is performing the search. </param>
        /// <param name="clause"> that defines the name and operator for which we want to find a clause validator, must not be null.
        /// </param>
        /// <returns> the validators associated with this clause, or empty list if the lookup failed. </returns>
        ICollection<IClauseValidator> GetClauseValidator(User searcher, ITerminalClause clause);

        /// <summary>
        /// Fetches the associated ClauseValidators for the provided WasClause. The returned value is based on
        /// the clause's name and the <see cref="com.atlassian.query.operator.Operator"/> that is associated with the
        /// provided clause. Multiple values may be returned for custom fields.
        /// </summary>
        /// <param name="searcher"> the user who is performing the search. </param>
        /// <param name="clause"> that defines the name and operator for which we want to find a clause validator, must not be null.
        /// </param>
        /// <returns> the validators associated with this clause, or empty list if the lookup failed. </returns>
        ICollection<IClauseValidator> GetClauseValidator(User searcher, IWasClause clause);

        /// <summary>
        /// Fetches the associated ClauseValidators for the provided ChangedClause.
        /// </summary>
        /// <param name="searcher"> the user who is performing the search. </param>
        /// <param name="clause"> that defines the field
        /// </param>
        /// <returns> the validators associated with this clause, or empty list if the lookup failed. </returns>
        ChangedClauseValidator GetClauseValidator(User searcher, IChangedClause clause);
    }
}