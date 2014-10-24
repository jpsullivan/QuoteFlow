using QuoteFlow.Models.Search.Jql.Clauses;

namespace QuoteFlow.Models.Search
{
    /// <summary>
    /// Represents a JQL clause and how to process it. Fields may use these objects to register 
    /// new JQL clauses within QuoteFlow.
    /// </summary>
    public class ClauseRegistration
    {
        public IClauseHandler Handler { get; set; }

        public ClauseRegistration(IClauseHandler handler)
        {
            Handler = handler;
        }
    }
}