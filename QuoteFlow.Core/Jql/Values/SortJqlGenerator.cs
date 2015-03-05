using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Order;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Api.Jql.Values;

namespace QuoteFlow.Core.Jql.Values
{
    /// <summary>
    /// Builds sort JQL for each column displayed in the issue navigator.
    /// </summary>
    public class SortJqlGenerator : ISortJqlGenerator
    {
        public IJqlStringSupport JqlStringSupport { get; protected set; }
        public ISearchHandlerManager SearchHandlerManager { get; protected set; }

        public SortJqlGenerator(IJqlStringSupport jqlStringSupport, ISearchHandlerManager searchHandlerManager)
        {
            JqlStringSupport = jqlStringSupport;
            SearchHandlerManager = searchHandlerManager;
        }

        /// <summary>
        /// Generate the JQL to be contained in the column headers. This JQL is the 
        /// current query with an additional ORDER BY applied.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="fields"></param>
        /// <returns>The JQL to be contained in the column headers.</returns>
        public IDictionary<string, string> GenerateColumnSortJql(IQuery query, List<INavigableField> fields)
        {
            var columnSortJql = new Dictionary<string, string>();
            foreach (var navigableField in fields)
            {
                var orderBy = BuildOrderBy(query.OrderByClause, navigableField);
                columnSortJql[navigableField.Id] = JqlStringSupport.GenerateJqlString(new Api.Jql.Query.Query(query.WhereClause, orderBy, null));
            }

            return columnSortJql;
        }

        /// <summary>
        /// Adds (or updates) a field to the {@code ORDER BY} portion of a query.
        /// Maintains the name or cf[id] id of a custom field depending on which was supplied.
        /// 
        /// Switches the direction of the order by query to make it the opposite of the
        /// present direction.
        /// 
        /// Updates a field with no explicit ORDER BY to be opposite of the default.
        /// 
        /// The following show the result of adding "project" to different queries:
        /// 
        ///   ORDER BY priority ASC -> ORDER BY project ASC, priority ASC
        ///   ORDER BY project DESC -> ORDER BY project ASC
        /// </summary>
        /// <param name="orderBy">The existing <code>ORDER BY</code> that is to be updated.</param>
        /// <param name="field">The field that is to be added.</param>
        /// <returns>An updated <code>ORDER BY</code>.</returns>
        private OrderBy BuildOrderBy(IOrderBy orderBy, INavigableField field)
        {
            SortOrder columnDirection = SortOrderHelpers.ParseString(field.DefaultSortOrder);

            string columnName;

//            // Custom fields must be in the format "cf[id]".
//            bool isCustomField = field is CustomField;
//            if (isCustomField)
//            {
//                CustomField customField = (CustomField)field;
//                columnName = JqlCustomFieldId.ToString(customField.IdAsLong);
//            }
//            else
//            {
//                ICollection<ClauseNames> jqlClauseNames = searchHandlerManager.getJqlClauseNames(field.Id);
//                columnName = jqlClauseNames.Count > 0 ? jqlClauseNames.GetEnumerator().next().PrimaryName : field.Id;
//            }

            var jqlClauseNames = SearchHandlerManager.GetJqlClauseNames(field.Id);
            columnName = jqlClauseNames.Count > 0 ? jqlClauseNames.First().PrimaryName : field.Id;

            IList<SearchSort> searchSorts = new List<SearchSort>();

            // inverts the sort sort order if orderBy already contains a search sort for this field (either using its
            // primary clause name or any of the alternates).
            foreach (SearchSort searchSort in orderBy.SearchSorts)
            {
                string fieldInSearch = searchSort.Field;

                if (fieldInSearch.Equals(columnName) || (IsAlias(field.Id, fieldInSearch)))
                {
                    columnName = fieldInSearch;

                    SortOrder sortOrder = searchSort.Order ?? SortOrderHelpers.ParseString(field.DefaultSortOrder);

                    // If no sortOrder direction has been specified

                    // Switch the direction so that when the field is clicked the alternate to current is requested.
                    columnDirection = sortOrder == SortOrder.ASC ? SortOrder.DESC : SortOrder.ASC;
                }
                else
                {
                    searchSorts.Add(searchSort);
                }
            }

            searchSorts.Insert(0, new SearchSort(columnName, columnDirection));
            return new OrderBy(searchSorts);
        }

        /// <summary>
        /// Determines whether the <code>columnName</code> is an alias for a field by checking if it is among the
        /// JQL clause's names.
        /// </summary>
        /// <param name="fieldId"> of column being sorted </param>
        /// <param name="columnName"> field in search </param>
        /// <returns> columnName is alias for columnName </returns>
        private bool IsAlias(string fieldId, string columnName)
        {
            var clauseInformation = SystemSearchConstants.GetClauseInformationById(fieldId);
            return (clauseInformation != null && clauseInformation.JqlClauseNames.Contains(columnName));
        }
    }
}
