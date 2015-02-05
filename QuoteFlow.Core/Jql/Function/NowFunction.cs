using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// 
    /// @since v4.0
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

        public virtual MessageSet validate(User searcher, FunctionOperand operand, TerminalClause terminalClause)
        {
            // Now is always now so there is not much to validate
            return validateNumberOfArgs(operand, 0);
        }

        public virtual IList<QueryLiteral> getValues(QueryCreationContext queryCreationContext, FunctionOperand operand, TerminalClause terminalClause)
        {
            return Collections.singletonList(new QueryLiteral(operand, clock.CurrentDate.Time));
        }
    }
}
