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
        public IClause WhereClause { get; set; }
        public IOrderBy OrderByClause { get; set; }
        public string QueryString { get; set; }

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

        IClause IQuery.WhereClause()
        {
            return WhereClause;
        }

        IOrderBy IQuery.OrderByClause()
        {
            return OrderByClause;
        }

        string IQuery.QueryString()
        {
            return QueryString;
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

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }

            var that = (Query) o;

            if (OrderByClause != null ? !OrderByClause.Equals(that.OrderByClause) : that.OrderByClause != null)
            {
                return false;
            }
            if (QueryString != null ? !QueryString.Equals(that.QueryString) : that.QueryString != null)
            {
                return false;
            }
            if (WhereClause != null ? !WhereClause.Equals(that.WhereClause) : that.WhereClause != null)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int result = WhereClause != null ? WhereClause.GetHashCode() : 0;
            result = 31 * result + (OrderByClause != null ? OrderByClause.GetHashCode() : 0);
            result = 31 * result + (QueryString != null ? QueryString.GetHashCode() : 0);
            return result;
        }
    }
}