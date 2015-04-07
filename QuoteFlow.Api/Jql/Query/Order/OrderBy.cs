using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteFlow.Api.Jql.Query.Order
{
    public class OrderBy : IOrderBy
    {
        public List<SearchSort> SearchSorts { get; private set; }

        public static readonly OrderBy NoOrder = new OrderBy(new List<SearchSort>());

		public OrderBy(params SearchSort[] searchSorts) : this(searchSorts.ToList())
		{
		}

		public OrderBy(ICollection<SearchSort> searchSorts)
		{
			SearchSorts = searchSorts.ToList();
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			if (SearchSorts.Count > 0)
			{
				sb.Append("order by ");
			}
		    sb.Append(String.Join(",", SearchSorts));
			return sb.ToString();
		}

		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (o == null || this.GetType() != o.GetType())
			{
				return false;
			}

			var orderBy = (OrderBy) o;

            if (!SearchSorts.SequenceEqual(orderBy.SearchSorts))
			{
				return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			return SearchSorts.GetHashCode();
		}
    }
}