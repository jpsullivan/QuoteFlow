using System;
using System.Collections.Generic;
using Lucene.Net.Index;
using Lucene.Net.Search;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Filters;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Util.Lucene;

namespace QuoteFlow.Core.Jql.Query
{
    /// <summary>
    /// Factory class for validating and building the Lucene Changed query.
    /// 
    /// @since v5.0
    /// </summary>
    public class ChangedClauseQueryFactory
    {
        private readonly ISearchProviderFactory _searchProviderFactory;
        private readonly HistoryPredicateQueryFactory _changedPredicateQueryFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchProviderFactory">Factory for retrieving the history search provider.</param>
        /// <param name="changedPredicateQueryFactory">Returns queries for the predicates.</param>
        public ChangedClauseQueryFactory(ISearchProviderFactory searchProviderFactory, HistoryPredicateQueryFactory changedPredicateQueryFactory)
        {
            _searchProviderFactory = searchProviderFactory;
            _changedPredicateQueryFactory = changedPredicateQueryFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searcher">The <see cref="User"/> representing the current searcher.</param>
        /// <param name="clause">The search clause, for instance "Status was Open".</param>
        /// <returns><see cref="QueryFactoryResult"/> that wraps the lucene query.</returns>
        public virtual QueryFactoryResult Create(User searcher, IChangedClause clause)
        {
            ConstantScoreQuery issueQuery;
            var changedQuery = MakeQuery(searcher, clause);
            IndexSearcher historySearcher = _searchProviderFactory.GetSearcher(SearchProviderTypes.CHANGE_HISTORY_INDEX);
            var collector = new AssetIdCollector(historySearcher.IndexReader);
            try
            {
//                if (log.DebugEnabled)
//                {
//                    log.debug("Running Changed query (" + clause + "): " + changedQuery);
//                }

                historySearcher.Search(changedQuery, collector);
                ISet<string> queryIds = collector.AssetIds;
                ISet<string> assetIds;
                if (clause.Operator == Operator.CHANGED)
                {
                    assetIds = queryIds;
                }
                else
                {
                    ISet<string> allAssetIds = collector.AllAssetIds;
                    foreach (var assetId in allAssetIds)
                    {
                        allAssetIds.Remove(assetId);
                    }
                    assetIds = allAssetIds;
                }

//                if (log.DebugEnabled)
//                {
//                    log.debug("History query returned: " + issueIds);
//                }

                issueQuery = new ConstantScoreQuery(new AssetIdFilter(assetIds));
            }
            catch (Exception e)
            {
                throw e;
            }
            return new QueryFactoryResult(issueQuery);

        }

        private global::Lucene.Net.Search.Query MakeQuery(User searcher, IChangedClause clause)
        {
            BooleanQuery outerQuery = new BooleanQuery();
            BooleanQuery changedQuery = new BooleanQuery();
            var toQuery = CreateQuery(clause, DocumentConstants.ChangeFrom);
            if (clause.Predicate == null)
            {
                changedQuery.Add(toQuery, Occur.SHOULD);
            }
            else
            {
                var predicateQuery = _changedPredicateQueryFactory.MakePredicateQuery(searcher, clause.Field.ToLower(), clause.Predicate, true);
                changedQuery.Add(predicateQuery, Occur.MUST);
                changedQuery.Add(toQuery, Occur.MUST);
            }
            outerQuery.Add(changedQuery, Occur.SHOULD);
            return outerQuery;
        }


        private PrefixQuery CreateQuery(IChangedClause clause, string documentField)
        {
            return ConstantScorePrefixQuery.Build(new Term(clause.Field.ToLower() + "." + documentField, "ch-"));
        }
    }
}