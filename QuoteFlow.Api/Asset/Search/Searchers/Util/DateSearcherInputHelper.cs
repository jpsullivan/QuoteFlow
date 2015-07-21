using System;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Util;

namespace QuoteFlow.Api.Asset.Search.Searchers.Util
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
                throw new ArgumentNullException(nameof(jqlDateSupport));
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