using System;
using QuoteFlow.Models.Search.Jql.Query.Clause;

namespace QuoteFlow.Infrastructure.Exceptions.Search
{
    [Serializable]
    public class ClauseTooComplexSearchException : Exception
    {
        public ClauseTooComplexSearchException(IClause clause) 
            : base("The following query was too complex to generate a query from: " + clause.ToString())
        {
            Clause = clause;
        }

        public IClause Clause { get; private set; }
    }
}