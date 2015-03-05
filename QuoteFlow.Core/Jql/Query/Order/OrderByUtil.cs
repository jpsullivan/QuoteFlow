using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Order;
using QuoteFlow.Api.Jql.Values;

namespace QuoteFlow.Core.Jql.Query.Order
{
    public class OrderByUtil : IOrderByUtil
    {
        #region DI

        public ISearchHandlerManager SearchHandlerManager { get; protected set; }
        public IFieldManager FieldManager { get; protected set; }
        public ISortJqlGenerator SortJqlGenerator { get; protected set; }

        public OrderByUtil(ISearchHandlerManager searchHandlerManager, IFieldManager fieldManager, ISortJqlGenerator sortJqlGenerator)
        {
            SearchHandlerManager = searchHandlerManager;
            FieldManager = fieldManager;
            SortJqlGenerator = sortJqlGenerator;
        }

        #endregion

        public SortBy GenerateSortBy(IQuery query)
        {
            var orderByClause = query.OrderByClause;
            if (orderByClause == null) return null;

            var searchSorts = orderByClause.SearchSorts;
            if (!searchSorts.Any()) return null;

            SearchSort searchSort = searchSorts.First();

            // this may return multiple field IDs if there are multiple custom fields that have the same name. in
            // that case we just consider that the search results are sorted by the 1st field that has the name
            var fieldIds = SearchHandlerManager.GetFieldIds(searchSort.Field);
            if (!fieldIds.Any()) return null;

            string fieldId = fieldIds.First();
            var field = FieldManager.GetField(fieldId);

            string order = searchSort.GetOrder();
            if (string.IsNullOrEmpty(order))
            {
                // if no order is provided, return the default sort order. it's likely that I'm being overly
                // defensive here, but I'm not sure if it's possible for this to *not* be navigable.
                var navigableField = field as INavigableField;
                order = navigableField != null ? navigableField.DefaultSortOrder : "ASC";
            }

            string sortJql = null;
            if (field is INavigableField)
            {
                sortJql = SortJqlGenerator.GenerateColumnSortJql(query, new List<INavigableField> {(INavigableField) field})[fieldId];
            }

            return new SortBy(field.Id, field.Name, order, sortJql);
        }
    }
}
