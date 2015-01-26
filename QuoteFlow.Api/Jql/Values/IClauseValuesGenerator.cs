using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Jql.Values
{
    /// <summary>
    /// Generates the possible values for a field.
    /// </summary>
    public interface IClauseValuesGenerator
    {
        /// <summary>
        /// Will return a string representation of only the possible values that match the value prefix
        /// for this clause.  This should not return more than specified in <see cref="maxNumResults"/>.
        /// If it is possible this should use the <see cref="maxNumResults"/> to efficiently generate the results.
        /// 
        /// The contract of this method is that if the <see cref="valuePrefix"/> exactly (minus case) matches
        /// the suggestion then we suggest it.  This will allow users to verify in their own minds that even
        /// though they have typed the full value, it is still valid.
        /// </summary>
        /// <param name="searcher">The user performing the search.</param>
        /// <param name="jqlClauseName">The jql clause name that was entered by the user. Represents the 
        /// identifier that was used to find this values generator.</param>
        /// <param name="valuePrefix">The portion of the value that has already been preovided by the user.</param>
        /// <param name="maxNumResults">The max number of results to return.</param>
        /// <returns>A string value of the clause values that match the provided value prefix.</returns>
        ClauseValueResults GetPossibleValues(User searcher, string jqlClauseName, string valuePrefix, int maxNumResults);
    }
}