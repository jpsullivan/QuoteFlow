using System;
using System.Text;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;

namespace QuoteFlow.Models.Search.Jql.Query
{
    /// <summary>
    /// Defines a structured graph of objects that can be used to represent query.
    /// </summary>
    public class Query : IQuery
    {
        private readonly IClause whereClause;
        private readonly OrderBy orderByClause;
        private readonly string queryString;

        public Query() : this(null, null, null)
		{
		}

        public Query(IClause whereClause)
            : this(whereClause, new OrderByImpl(), null)
		{
		}

        public Query(IClause whereClause, string originalQuery)
            : this(whereClause, new OrderByImpl(), originalQuery)
		{
		}

        public Query(IClause whereClause, OrderBy orderByClause, string originalQuery)
		{
			this.whereClause = whereClause;
			this.queryString = originalQuery;
			this.orderByClause = orderByClause;
		}

        public virtual IClause WhereClause
		{
			get
			{
				return whereClause;
			}
		}

		public virtual OrderBy OrderByClause
		{
			get
			{
				return orderByClause;
			}
		}

		public virtual string QueryString
		{
			get
			{
				return queryString;
			}
		}

        IClause IQuery.WhereClause()
        {
            throw new NotImplementedException();
        }

        OrderBy IQuery.OrderByClause()
        {
            throw new NotImplementedException();
        }

        string IQuery.QueryString()
        {
            throw new NotImplementedException();
        }

		public override string ToString()
		{
			var builder = new StringBuilder();
			if (whereClause != null)
			{
				builder.Append(whereClause.ToString());
			}

			if (orderByClause != null && orderByClause.SearchSorts.Count > 0)
			{
				if (builder.Length > 0)
				{
					builder.Append(" ");
				}
				builder.Append(orderByClause.ToString());
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

            if (orderByClause != null ? !orderByClause.Equals(that.orderByClause) : that.orderByClause != null)
            {
                return false;
            }
            if (queryString != null ? !queryString.Equals(that.queryString) : that.queryString != null)
            {
                return false;
            }
            if (whereClause != null ? !whereClause.Equals(that.whereClause) : that.whereClause != null)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int result = whereClause != null ? whereClause.GetHashCode() : 0;
            result = 31 * result + (orderByClause != null ? orderByClause.GetHashCode() : 0);
            result = 31 * result + (queryString != null ? queryString.GetHashCode() : 0);
            return result;
        }
    }
}