using QuoteFlow.Models.Search.Jql.Clauses;
using QuoteFlow.Models.Search.Jql.Context;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Validator;

namespace QuoteFlow.Models.Search.Jql.Values
{
    /// <summary>
    /// A container for all the objects needed to process a Jql clause.
    /// </summary>
    public class ClauseHandler : IClauseHandler
    {
        public IClauseInformation Information { get; private set; }
        public IClauseQueryFactory Factory { get; private set; }
        public IClauseValidator Validator { get; private set; }
        public IClauseContextFactory ClauseContextFactory { get; private set; }

        public ClauseHandler(IClauseInformation information, IClauseQueryFactory factory, IClauseValidator validator, IClauseContextFactory clauseContextFactory)
        {
            Information = information;
            Factory = factory;
            Validator = validator;
            ClauseContextFactory = clauseContextFactory;
        }
    }
}