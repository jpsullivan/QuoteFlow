using System;
using System.Collections.Generic;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Core.Jql.Function
{
    /// <summary>
    /// Function that produces the current date as the value.
    /// </summary>
    public class NowFunction : AbstractDateFunction
    {
        public const string FUNCTION_NOW = "now";

        internal NowFunction(Clock clock, TimeZoneManager timeZoneManager)
            : base(clock, timeZoneManager)
        {
        }

        public NowFunction(TimeZoneManager timeZoneManager)
            : base(timeZoneManager)
        {
        }

        public virtual IMessageSet Validate(User searcher, FunctionOperand operand, TerminalClause terminalClause)
        {
            // Now is always now so there is not much to validate
            return ValidateNumberOfArgs(operand, 0);
        }

        public virtual IList<QueryLiteral> GetValues(QueryCreationContext queryCreationContext, FunctionOperand operand, TerminalClause terminalClause)
        {
            return new List<QueryLiteral> { new QueryLiteral(operand, DateTime.UtcNow )};
        }
    }
}
