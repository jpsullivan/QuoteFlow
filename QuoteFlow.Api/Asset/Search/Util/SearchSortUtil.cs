using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Order;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Asset.Search.Util
{
    public class SearchSortUtil : ISearchSortUtil
    {
        public string SorterOrder { get { return "sorter/order"; } }
        public string SorterField { get { return "sorter/field"; } }

        public ISearchHandlerManager SearchHandlerManager { get; protected set; }
        public IFieldManager FieldManager { get; protected set; }
        
        public static SearchSort DefaultKeySort = new SearchSort(SystemSearchConstants.ForAssetId().JqlClauseNames.PrimaryName, SortOrder.DESC);

        public SearchSortUtil(ISearchHandlerManager searchHandlerManager, IFieldManager fieldManager)
        {
            SearchHandlerManager = searchHandlerManager;
            FieldManager = fieldManager;
        }

        public IList<SearchSort> MergeSearchSorts(User user, ICollection<SearchSort> newSorts, ICollection<SearchSort> oldSorts, int maxLength)
        {
            if (newSorts == null)
            {
                throw new ArgumentNullException("newSorts");
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts any new sorts which refer to the same clause as any existing old sorts to use the old sort's name if it
        /// is different to the new name.
        /// 
        /// For example, if your new sort is <code>Key DESC</code> and you have an old sort <code>issueKey ASC</code>, the new
        /// sort will be converted to be <code>issueKey DESC</code>.
        /// 
        /// The point here is to preserve old JQL strings as much as possible (but only when it is functionally identical).
        /// For more info see http://jira.atlassian.com/browse/JRA-17908.
        /// </summary>
        /// <param name="user">The user performing the search.</param>
        /// <param name="newSorts">The new sorts. must not be null.</param>
        /// <param name="oldSorts">The old sorts. must not be null.</param>
        /// <returns>The converted new sorts; never null.</returns>
        internal virtual ICollection<SearchSort> ConvertNewSortsToKeepOldSortNames(User user, IEnumerable<SearchSort> newSorts, IEnumerable<SearchSort> oldSorts)
        {
            var convertedSorts = new List<SearchSort>();
            foreach (SearchSort newSort in newSorts)
            {
                var newHandlers = SearchHandlerManager.GetClauseHandler(user, newSort.Field).ToList();
                if (newHandlers.Count != 1)
                {
                    convertedSorts.Add(newSort);
                    continue;
                }
                newHandlers.GetEnumerator().MoveNext();
                IClauseHandler newHandler = newHandlers.GetEnumerator().Current;

                SearchSort convertedSort = newSort;
                foreach (SearchSort oldSort in oldSorts)
                {
                    var oldHandlers = SearchHandlerManager.GetClauseHandler(user, oldSort.Field).ToList();
                    if (oldHandlers.Count != 1) continue;

                    oldHandlers.GetEnumerator().MoveNext();
                    IClauseHandler handler = oldHandlers.GetEnumerator().Current;
                    if (handler == null) continue;
                    if (newHandler.Information.JqlClauseNames.Equals(handler.Information.JqlClauseNames))
                    {
                        if (!newSort.Field.Equals(oldSort.Field))
                        {
                            convertedSort = new SearchSort(oldSort.Field, newSort.Order);
                        }
                    }
                }
                convertedSorts.Add(convertedSort);
            }
            return convertedSorts;
        }

        public IEnumerable<SearchSort> GetSearchSorts(IQuery query)
        {
            IList<SearchSort> sorts;
            // when the whole query is null we fall back to the default sorts.
            if (query == null)
            {
                sorts = new List<SearchSort>();
            }
            else
            {
                // when the order by clause is null we use this condition to force us to use not sorts for our query at all.
                if (query.OrderByClause == null)
                {
                    return null;
                }
                sorts = query.OrderByClause.SearchSorts;
            }

            // This is a special case where we want to put in the default QuoteFlow sorts
            if (sorts.Count == 0)
            {
                //If we have a free text query, then we don't want any sorts so that Lucene's rank will work for us.
                if (query == null || !FreeTextVisitor.ContainsFreeTextCondition(query.WhereClause))
                {
                    //By default we sort by the "Issue Key" when there is no text searcher to rank the results.
                    sorts = new List<SearchSort> { DefaultKeySort };
                }
            }

            return sorts;
        }

        public IOrderBy GetOrderByClause(IDictionary parameterMap)
        {
            IList<SearchSort> searchSorts = new List<SearchSort>();
            int minLength;
            var orders = ParameterUtils.GetListParam(parameterMap, SearchSorters.Order);
            var fields = ParameterUtils.GetListParam(parameterMap, SearchSorters.Field);

            // get min length
            // loop for i = 0 to min.length
            // add sort for order[i] && field[i]
            if ((orders == null) || (fields == null))
            {
                minLength = 0;
            }
            else
            {
                minLength = Math.Min(orders.Count, fields.Count);
            }

            for (int i = 0; i < minLength; i++)
            {
                string order = (string)orders[i];
                string field = (string)fields[i];

                if ((order != null) && (field != null))
                {
                    // Lets convert the field name into a JQL primary clause name
                    ICollection<ClauseNames> matchingClauseNames = SearchHandlerManager.GetJqlClauseNames(field);
                    // We only need to take the first ClauseNames since they will all resolve to the same field, which, in
                    // the end, will resolve to the same sort
                    if (matchingClauseNames.Count > 0)
                    {
                        if (FieldManager.IsNavigableField(field))
                        {
                            matchingClauseNames.GetEnumerator().MoveNext();
                            var names = matchingClauseNames.GetEnumerator().Current;
                            searchSorts.Add(new SearchSort(order, names.PrimaryName));
                        }
                        else
                        {
//                            log.warn("Unable to create a search sort for the field name '" + field + "' as the field is able to be sorted.");
                        }
                    }
                    else
                    {
//                        log.warn("Unable to create a search sort for field name '" + field + "' as there is no associated JQL clause name.");
                    }
                }
            }

            return new OrderBy(searchSorts);
        }

        public IList<SearchSort> ConcatSearchSorts(IEnumerable<SearchSort> newSorts, IEnumerable<SearchSort> oldSorts, int maxLength)
        {
            if (newSorts == null)
            {
                throw new ArgumentNullException("newSorts");
            }

            // like merge, except we don't bother checking for duplicates - just join the two collections together (new then old)
            var calcSorts = new List<SearchSort>();
            calcSorts.AddRange(newSorts);
            calcSorts.AddRange(oldSorts);

            return calcSorts;
        }

        public IEnumerable<string> GetSearchSortDescriptions(SearchRequest searchRequest, User searcher)
        {
            if (searchRequest == null)
            {
                throw new ArgumentNullException("searchRequest");
            }

            IList<string> searchSortDescriptions = new List<string>();

            IEnumerable<SearchSort> searchSorts = this.GetSearchSorts(searchRequest.Query);
            foreach (SearchSort searchSort in searchSorts)
            {
                string sortClauseName = searchSort.Field;

                var fieldIds = new List<string>(SearchHandlerManager.GetFieldIds(searcher, sortClauseName));
                // sort to get consistent ordering of fields for clauses with multiple fields
                fieldIds.Sort();

                foreach (string fieldId in fieldIds)
                {
                    IField field = FieldManager.GetField(fieldId);
                    if (field != null)
                    {
                        StringBuilder description = new StringBuilder();
                        description.Append(field.NameKey);
                        string orderDescription = GetSearchSortOrderDescription(searchSort.Order.ToString(), field);
                        if (orderDescription.HasValue())
                        {
                            description.Append(" ").Append(orderDescription);
                        }
                        searchSortDescriptions.Add(description.ToString());
                    }
                    else
                    {
//                        log.info("Field '" + sortClauseName + "' is invalid as a search sort in SearchRequest " + searchRequest);
                    }
                }
            }

            // now we know that every element in the list is valid, add in the ", then" to all but the last string
            for (int i = 0; i < searchSortDescriptions.Count; i++)
            {
                string description = searchSortDescriptions[i];
                if (i >= searchSortDescriptions.Count - 1) continue;

                string newDescription = description + ", " + "navigator.hidden.sortby.then";
                searchSortDescriptions[i] = newDescription;
            }
            return searchSortDescriptions;

        }

        private static string GetSearchSortOrderDescription(string searchSortOrder, IField field)
        {
            if (!(field is INavigableField))
            {
                return "";
            }

            var navigableField = (INavigableField) field;

            searchSortOrder = string.IsNullOrWhiteSpace(searchSortOrder) ? navigableField.DefaultSortOrder : searchSortOrder;
            if (NavigableFieldOrder.Descending == searchSortOrder)
            {
                return "navigator.hidden.sortby.descending";
            }

            return "navigator.hidden.sortby.ascending";
        }
    }
}