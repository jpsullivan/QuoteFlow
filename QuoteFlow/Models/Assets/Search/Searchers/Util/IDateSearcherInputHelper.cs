using System.Collections.Generic;
using QuoteFlow.Models.Search.Jql.Query.Clause;

namespace QuoteFlow.Models.Assets.Search.Searchers.Util
{
    /// <summary>
    /// Inteface for classues that parse JQL clauses and determine if they are suitable for usage in the Navigator or Search URL.
    /// </summary>
    public interface IDateSearcherInputHelper
    {
        /// <summary>
        /// Take the passed clause and try and get the equivalent navigator parameters.
        /// 
        /// Note: this also performs a validity check on the structure of the clause to determine if it fits for the Navigator.
        /// Therefore, it is not required to check this before the call is made.
        /// </summary>
        /// <param name="clause"> the clause to convert. </param>
        /// <param name="user"> the user trying to convert the clause. </param>
        /// <param name="allowTimeComponent"> if true, date values which aren't midnight dates will be returned as midnight
        /// dates (thereby losing precision) </param>
        /// <returns> on success a map of navigator param -> value, or null on failure. The map will only contain the params
        /// that were present in the clause. </returns>
        ConvertClauseResult ConvertClause(IClause clause, User user, bool allowTimeComponent);
    }

    public class ConvertClauseResult
    {
        public ConvertClauseResult(IDictionary<string, string> fields, bool fitsFilterForm)
		{
			Fields = fields;
            FitsFilterForm = fitsFilterForm;
		}

        public IDictionary<string, string> Fields { get; private set; }
        public bool FitsFilterForm { get; private set; }
    }
}