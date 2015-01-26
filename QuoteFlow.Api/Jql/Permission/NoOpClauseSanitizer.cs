using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Jql.Permission
{
    /// <summary>
    /// A no-op sanitizer that simply returns the input clause.
    /// </summary>
    public class NoOpClauseSanitizer : IClauseSanitizer
    {
        public static readonly NoOpClauseSanitizer NoopClauseSanitizer = new NoOpClauseSanitizer();

        // shouldn't need construction
        private NoOpClauseSanitizer() { }

        public IClause Sanitize(User user, ITerminalClause clause)
        {
            return clause;
        }
    }
}