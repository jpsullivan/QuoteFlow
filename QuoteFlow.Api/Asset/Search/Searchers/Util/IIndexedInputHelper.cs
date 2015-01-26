using System.Collections.Generic;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset.Search.Searchers.Util
{
    /// <summary>
	/// Provides methods for retreiving the Navigator or index representations of the values in a query clause, be they
	/// index values, functions or string values.
	/// 
	/// NOTE: this method will give back the values it finds in the named clauses and does not take into account
	/// the affects of the surrounding logical search query (i.e. AND, NOT, OR clauses that the clause may be
	/// contained in).
	/// 
	/// @since v4.0
	/// </summary>
	public interface IIndexedInputHelper
	{
		/// <summary>
		/// Retrieves the index values for the clauses in the <seealso cref="SearchRequest"/>. Function Operands are expanded to their
		/// values.
		/// </summary>
		/// <param name="searcher"> the user running the search </param>
		/// <param name="jqlClauseNames"> the names of the clauses on which to retreive the values. </param>
		/// <param name="query"> the search criteria used to populate the field values holder. </param>
		/// <returns> a set of strings containing the index values of the clause values. Never null. </returns>
		ISet<string> GetAllIndexValuesForMatchingClauses(User searcher, ClauseNames jqlClauseNames, IQuery query);

		/// <summary>
		/// Retreives the navigator id values for the values in the clauses. If there is a flag associated with a function operand
		/// then that flag is returned, otherwise the function operand is expanded to its index values.
		/// </summary>
		/// <param name="searcher"> the user running the search </param>
		/// <param name="jqlClauseNames"> the names of the clauses on which to retreive the values. </param>
		/// <param name="query"> the search criteria used to populate the field values holder. </param>
		/// <returns> a set of strings containing the navigator values of the clause values. Never Null. </returns>
		ISet<string> GetAllNavigatorValuesForMatchingClauses(User searcher, ClauseNames jqlClauseNames, IQuery query);

		/// <summary>
		/// Converts a set of Navigator value strings into a Clause that will match at least one of the specified values for
		/// the given field.
		/// <p/>
		/// Note: where possible, the helper should try to create a clause, even when the value strings do not make sense
		/// in the given domain. That is, it is preferred that a non-validating clause gets created than no clause at all.
		/// </summary>
		/// <param name="jqlClauseName"> the name of the clause to generate </param>
		/// <param name="values"> a set of Navigator value strings; may contain flag values. May not be null. </param>
		/// <returns> a clause that will match any of the values specified; null if no values were specified. </returns>
		IClause GetClauseForNavigatorValues(string jqlClauseName, IEnumerable<string> values);
	}

}