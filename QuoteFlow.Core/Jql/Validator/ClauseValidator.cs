using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Core.Jql.Validator
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