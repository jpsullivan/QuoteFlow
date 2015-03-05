﻿using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Nav;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Asset.Search.Util;
using QuoteFlow.Api.Configuration;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Order;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Models.ViewModels.Assets.Nav;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Jql.Builder;

namespace QuoteFlow.Core.Asset.Nav
{
    public class AssetTableService : IAssetTableService
    {
        private const string ColumnNames = "columnNames";
        private const int MaxJqlErrors = 10;

        #region DI

        public IFieldManager FieldManager { get; protected set; }
        public ISearchService SearchService { get; protected set; }
        public ISearchSortUtil SearchSortUtil { get; protected set; }
        public ISearchHandlerManager SearchHandlerManager { get; protected set; }

        public AssetTableService(IFieldManager fieldManager, ISearchService searchService, ISearchSortUtil searchSortUtil, ISearchHandlerManager searchHandlerManager)
        {
            FieldManager = fieldManager;
            SearchService = searchService;
            SearchSortUtil = searchSortUtil;
            SearchHandlerManager = searchHandlerManager;
        }

        #endregion

        public AssetTableViewModel GetIssueTableFromFilterWithJql(string filterId, string jql, IAssetTableServiceConfiguration config,
            bool isStableSearchFirstHit)
        {
            if (filterId.IsNullOrWhiteSpace())
            {
                return GetAssetTableFromJql(jql, config, isStableSearchFirstHit);
            }

            throw new NotImplementedException();
        }

        public AssetTableViewModel GetAssetTableFromAssetIds(string filterId, string jql, List<int> ids,
            IAssetTableServiceConfiguration config)
        {
            throw new NotImplementedException();
        }

        internal AssetTableViewModel GetAssetTableFromJql(string jql, IAssetTableServiceConfiguration config,
            bool returnMatchingAssetIds)
        {
            // parse the JQL into a Query object (and to validate it).
            var parseResult = SearchService.ParseQuery(new User(), jql);
            if (!parseResult.IsValid())
            {
                //return BuildJqlErrorViewModel(parseResult.Errors);
                return null;
            }

            var searchRequest = new SearchRequest(parseResult.Query);
            return GetAssetTable(config, parseResult.Query, returnMatchingAssetIds, searchRequest);
        }

//        /// <summary>
//        /// Builds an "error" asset table model containing the given errors. If no errors, uses a generic
//        /// "JQL is invalid" error message.
//        /// </summary>
//        /// <param name="messageSet">The errors to include.</param>
//        /// <returns>An "error" asset table vm containing errors.</returns>
//        private AssetTableViewModel BuildJqlErrorViewModel(IMessageSet messageSet)
//        {
//            ErrorCollection errors = new SimpleErrorCollection();
//            IEnumerable<string> errorMessages = Iterables.limit(messageSet.ErrorMessages, MaxJqlErrors);
//
//            foreach (string error in errorMessages)
//            {
//                errors.addErrorMessage(error);
//            }
//
//            if (!errors.hasAnyErrors())
//            {
//                errors.addErrorMessage(authenticationContext.I18nHelper.getText("jql.parse.unknown.no.pos"));
//            }
//
//            return new AssetTableViewModel(errors);
//        }

        /// <summary>
        /// Attempt to construct an <see cref="AssetTable"/>.
        /// </summary>
        /// <param name="configuration">The service configuration to use.</param>
        /// <param name="query">The query whose results will form the table content.</param>
        /// <param name="returnIssueIds">Whether the issues' IDs should be returned.</param>
        /// <param name="searchRequest">The search request being executed (may differ from {@code query}).</param>
        /// <returns>The result of attempting to create an <see cref="AssetTable"/> from the given arguments.</returns>
        internal virtual AssetTableViewModel GetAssetTable(IAssetTableServiceConfiguration configuration, IQuery query, bool returnIssueIds, SearchRequest searchRequest)
        {
            if (searchRequest == null)
            {
                searchRequest = new SearchRequest(query);
            }

            var queryWithOrderBy = AddOrderByToSearchRequest(query, configuration.SortBy);
            PreferredLayoutKey = configuration;

            return CreateIssueTableFromCreator(issueTableCreatorFactory.getNormalIssueTableCreator(configuration, queryWithOrderBy, returnIssueIds, searchRequest, authenticationContext.LoggedInUser));
        }

        internal virtual IQuery AddOrderByToSearchRequest(IQuery preOrderByQuery, string sortBy)
        {
            if (sortBy.IsNullOrWhiteSpace())
            {
                return preOrderByQuery;
            }

            string sortDirection = null;
            if (sortBy.EndsWith(":DESC") || sortBy.EndsWith(":ASC"))
            {
                sortDirection = sortBy.Substring(sortBy.LastIndexOf(':') + 1);
                sortBy = sortBy.Substring(0, sortBy.LastIndexOf(':'));
            }

            var queryBuilder = JqlQueryBuilder.NewBuilder(preOrderByQuery);

            string[] sortArray = { sortDirection };
            string[] fieldArray = { sortBy };

            var @params = new Dictionary<string, string[]>
            {
                {SearchSortUtil.SorterOrder, sortArray},
                {SearchSortUtil.SorterField, fieldArray}
            };

            IOrderBy newOrder = SearchSortUtil.GetOrderByClause(@params);
            IOrderBy oldOrder = queryBuilder.OrderBy().BuildOrderBy();

            User user = authenticationContext.LoggedInUser;
            IList<SearchSort> newSearchSorts = newOrder.SearchSorts;
            IList<SearchSort> oldSearchSorts = oldOrder.SearchSorts;
            IList<SearchSort> sorts = SearchSortUtil.MergeSearchSorts(user, newSearchSorts, oldSearchSorts, 3);

            queryBuilder.OrderBy().SetSorts(sorts);
            return queryBuilder.BuildQuery();
        }

        internal virtual void ValidateColumnNames(IList<string> columnNames)
        {
            if (columnNames == null || columnNames.Count <= 0) return;

            try
            {
                IList<string> fieldsNotFound = new List<string>();
                User user = authenticationContext.LoggedInUser;
                var availableFields = FieldManager.GetAvailableNavigableFields(user);
                    
                foreach (string columnName in columnNames)
                {
                    // skip the --Default-- column if present
                    if (columnName.Equals("--Default--", StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }

                    bool found = availableFields.Any(availableField => columnName.Equals(availableField.Id));
                    if (!found)
                    {
                        fieldsNotFound.Add(columnName);
                    }
                }

                if (fieldsNotFound.Count > 0)
                {
                    var fieldsNotFoundString = String.Join(", ", fieldsNotFound);
                    //errors.AddError(ColumnNames, string.Format("The following columns are invalid: {0}", fieldsNotFoundString));
                }
            }
            catch (FieldException e)
            {
                throw new Exception(e);
            }
        }

        private IAssetTableServiceConfiguration PreferredLayoutKey
        {
            set
            {
                if (value.LayoutKey == null)
                {
                    value.LayoutKey = "split-view";
                }
            }
        }
    }
}
