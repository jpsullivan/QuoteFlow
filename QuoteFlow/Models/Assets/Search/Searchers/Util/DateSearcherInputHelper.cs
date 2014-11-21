using System;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Util;

namespace QuoteFlow.Models.Assets.Search.Searchers.Util
{
    /// <summary>
    /// Helper class to parse JQL clauses and determine if they are suitable for usage in the Navigator or Search URL.
    /// </summary>
    public class DateSearcherInputHelper : AbstractDateSearchInputHelper
    {
        private readonly IJqlDateSupport jqlDateSupport;

        public DateSearcherInputHelper(DateSearcherConfig config, IJqlOperandResolver operandResolver, IJqlDateSupport jqlDateSupport)
            : base(config, operandResolver)
		{
            if (jqlDateSupport == null)
            {
                throw new ArgumentNullException("jqlDateSupport");
            }

			this.jqlDateSupport = jqlDateSupport;
		}

        internal override ParseDateResult GetValidNavigatorDate(QueryLiteral dateLiteral, bool allowTimeComponent)
		{
			DateTime date;
			if (dateLiteral.IntValue != null)
			{
				date = jqlDateSupport.ConvertToDate(dateLiteral.IntValue);
			}
			else if (dateLiteral.StringValue.HasValue())
			{
				date = jqlDateSupport.ConvertToDate(dateLiteral.StringValue);
			}
			else
			{
				return null;
			}

            return new ParseDateResult(true, date.ToString("yyyy-MM-dd"));
		}
    }
}