using System.Text;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Query.Order;

namespace QuoteFlow.Models.Search.Jql.Query
{
    /// <summary>
    /// Defines a structured graph of objects that can be used to represent query.
    /// </summary>
    public class Query : IQuery
    {
        public IClause WhereClause { get; private set; }
        public IOrderBy OrderByClause { get; private set; }
        public string QueryString { get; private set; }

        public Query() : this(null, null, null)
		{
		}

        public Query(IClause whereClause)
            : this(whereClause, new OrderBy(), null)
		{
		}

        public Query(IClause whereClause, string originalQuery)
            : this(whereClause, new OrderBy(), originalQuery)
		{
		}

        public Query(IClause whereClause, IOrderBy orderByClause, string originalQuery)
		{
			WhereClause = whereClause;
			QueryString = originalQuery;
			OrderByClause = orderByClause;
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			if (WhereClause != null)
			{
				builder.Append(WhereClause.ToString());
			}

			if (OrderByClause != null && OrderByClause.SearchSorts.Count > 0)
			{
				if (builder.Length > 0)
				{
					builder.Append(" ");
				}
				builder.Append(OrderByClause);
			}

			return builder.ToString();
		}
    }
}