using System;
using System.Collections.Generic;

namespace QuoteFlow.Api.Jql.Values
{
    public class ClauseValuesGeneratorResults
    {
        private readonly IList<ClauseValuesGeneratorResult> results;

        public ClauseValuesGeneratorResults(IEnumerable<ClauseValuesGeneratorResult> results)
		{
            if (results == null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            this.results = new List<ClauseValuesGeneratorResult>(results);
		}

        public virtual IList<ClauseValuesGeneratorResult> Results
		{
			get { return new List<ClauseValuesGeneratorResult>(); }
		}
    }
}