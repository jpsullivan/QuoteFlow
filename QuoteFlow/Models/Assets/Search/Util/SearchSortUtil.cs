using System;
using System.Collections;
using System.Collections.Generic;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Order;

namespace QuoteFlow.Models.Assets.Search.Util
{
    public class SearchSortUtil : ISearchSortUtil
    {
        public IList<SearchSort> mergeSearchSorts(User user, ICollection<SearchSort> newSorts, ICollection<SearchSort> oldSorts, int maxLength)
        {
            throw new NotImplementedException();
        }

        public IList<SearchSort> getSearchSorts(Query query)
        {
            throw new NotImplementedException();
        }

        public IOrderBy getOrderByClause(IDictionary parameterMap)
        {
            throw new NotImplementedException();
        }

        public IList<SearchSort> concatSearchSorts(ICollection<SearchSort> newSorts, ICollection<SearchSort> oldSorts, int maxLength)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> getSearchSortDescriptions(SearchRequest searchRequest, User searcher)
        {
            throw new NotImplementedException();
        }
    }
}