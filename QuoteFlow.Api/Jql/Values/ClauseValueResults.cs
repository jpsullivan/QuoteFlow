using System.Collections.Generic;

namespace QuoteFlow.Api.Jql.Values
{
    public class ClauseValueResults
    {
        public IEnumerable<ClauseValueResult> Results { get; set; }

        public ClauseValueResults(IEnumerable<ClauseValueResult> results)
        {
            Results = results;
        }
    }
}