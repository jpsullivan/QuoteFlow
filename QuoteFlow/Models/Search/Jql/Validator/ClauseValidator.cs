using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Search.Jql.Query.Clause;

namespace QuoteFlow.Models.Search.Jql.Validator
{
    public class ClauseValidator : IClauseValidator
    {
        public IMessageSet Validate(User searcher, ITerminalClause terminalClause)
        {
            var messages = new MessageSet();
            messages.AddErrorMessage(terminalClause.Name);
            return messages;
        }
    }
}