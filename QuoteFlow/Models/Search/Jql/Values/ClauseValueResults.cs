using System.Collections.Generic;

namespace QuoteFlow.Models.Search.Jql.Values
{
    public class ClauseValueResults
    {
        public IEnumerable<ClauseValueResults> Results { get; set; }

        public ClauseValueResults(IEnumerable<ClauseValueResults> results)
        {
            Results = results;
        }
    }
}