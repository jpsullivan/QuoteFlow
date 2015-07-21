using System;
using System.Collections.Generic;
using Lucene.Net.Index;
using Lucene.Net.Search;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Filters;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Jql.Operand;

namespace QuoteFlow.Core.Jql.Query
{
    /// <summary>
    /// Factory class for validating and building the Lucene Was query.
    /// </summary>
    public class WasClauseQueryFactory
    {
        public ISearchProviderFactory SearchProviderFactory { get; protected set; }
        public IJqlOperandResolver OperandResolver { get; protected set; }
        public HistoryPredicateQueryFactory WasPredicateQueryFactory { get; protected set; }
        public EmptyWasClauseOperandHandler EmptyWasClauseOperandHandler { get; protected set; }
        public ChangeHistoryFieldIdResolver ChangeHistoryFieldIdResolver { get; protected set; }

        /// <param name="searchProviderFactory">Factory for retrieving the history search provider</param>
        /// <param name="operandResolver">Resolves <see cref="Operand"/>  and retrieves their values</param>
        /// <param name="wasPredicateQueryFactory">Returns queries for the predicates</param>
        /// <param name="emptyWasClauseOperandHandler">Handler for WAS EMPTY queries</param>
        /// <param name="changeHistoryFieldIdResolver"></param>
        public WasClauseQueryFactory(ISearchProviderFactory searchProviderFactory, IJqlOperandResolver operandResolver, HistoryPredicateQueryFactory wasPredicateQueryFactory, EmptyWasClauseOperandHandler emptyWasClauseOperandHandler, ChangeHistoryFieldIdResolver changeHistoryFieldIdResolver)
        {
            SearchProviderFactory = searchProviderFactory;
            OperandResolver = operandResolver;
            ChangeHistoryFieldIdResolver = changeHistoryFieldIdResolver;
            WasPredicateQueryFactory = wasPredicateQueryFactory;
            EmptyWasClauseOperandHandler = emptyWasClauseOperandHandler;
        }

        protected WasClauseQueryFactory()
        {
        }

        /// <param name="searcher">The <see cref="User"/> representing the current searcher.</param>
        /// <param name="clause">The search cluase , for instance "Status was Open".</param>
        /// <returns><see cref="QueryFactoryResult"/> that wraps the  Lucene Query.</returns>
        public QueryFactoryResult Create(User searcher, IWasClause clause)
        {
            ConstantScoreQuery issueQuery;
            global::Lucene.Net.Search.Query historyQuery = MakeQuery(searcher, clause);
            IndexSearcher historySearcher = SearchProviderFactory.GetSearcher(SearchProviderTypes.CHANGE_HISTORY_INDEX);
            var collector = new AssetIdCollector(historySearcher.IndexReader);

            try
            {
                historySearcher.Search(historyQuery, collector);
                ISet<string> queryIds = collector.AssetIds;
                ISet<string> assetIds;
                if (clause.Operator == Operator.WAS || clause.Operator == Operator.WAS_IN)
                {
                    assetIds = queryIds;
                }
                else
                {
                    var allIssueIds = collector.AllAssetIds;
                    foreach (var issueId in allIssueIds)
                    {
                        allIssueIds.Remove(issueId);
                    }
                    assetIds = allIssueIds;
                }

                issueQuery = new ConstantScoreQuery(new AssetIdFilter(assetIds));
            }
            catch (Exception e)
            {
                throw e;
            }

            return new QueryFactoryResult(issueQuery);
        }

        private global::Lucene.Net.Search.Query MakeQuery(User searcher, IWasClause clause)
        {
            var outerWasQuery = new BooleanQuery();
            bool isEmptyOperand = clause.Operand is EmptyOperand;
            var literals = new List<QueryLiteral>();
            if (isEmptyOperand)
            {
                literals.AddRange(EmptyWasClauseOperandHandler.GetEmptyValue(clause));
            }
            else if (clause.Operand is MultiValueOperand)
            {
                literals.AddRange(OperandResolver.GetValues(searcher, clause.Operand, clause));
            }
            else if (clause.Operand is FunctionOperand)
            {
                literals.AddRange(OperandResolver.GetValues(searcher, clause.Operand, clause));
            }
            else
            {
                literals.Add(OperandResolver.GetSingleValue(searcher, clause.Operand, clause));
            }
            foreach (QueryLiteral literal in literals)
            {
                BooleanQuery wasQuery = new BooleanQuery();
                ICollection<string> ids = ChangeHistoryFieldIdResolver.ResolveIdsForField(clause.Field, literal, isEmptyOperand);
                if (ids == null || ids.Count == 0)
                {
                    if (literal != null)
                    {
                        string value = literal.IntValue?.ToString() ?? literal.StringValue;
                        // If we can't match the id to a current valid value then we just search with the literal. It may have been
                        // a valid value once upon a time, Of course we may still find nothing matches
                        TermQuery fromQuery = CreateTermQuery(clause, EncodeProtocol(value), DocumentConstants.ChangeFrom);
                        TermQuery toQuery = CreateTermQuery(clause, EncodeProtocol(value), DocumentConstants.ChangeTo);
                        //  searches for status was EMPTY will have null ids
                        if (value != null && clause.Predicate == null)
                        {
                            wasQuery.Add(fromQuery, Occur.SHOULD);
                        }
                        wasQuery.Add(toQuery, Occur.SHOULD);
                    }
                }
                else
                {
                    foreach (string id in ids)
                    {
                        TermQuery fromQuery = CreateTermQuery(clause, EncodeProtocolPreservingCase(id), DocumentConstants.OldValue);
                        TermQuery toQuery = CreateTermQuery(clause, EncodeProtocolPreservingCase(id), DocumentConstants.NewValue);
                        //  searches for status was EMPTY will have null ids
                        if (id != null && clause.Predicate == null)
                        {
                            wasQuery.Add(fromQuery, Occur.SHOULD);
                        }
                        wasQuery.Add(toQuery, Occur.SHOULD);
                    }
                }
                // JRADEV-7161 : need to should between each id, rather than at the end
                if (clause.Predicate != null)
                {
                    var wasPredicateQuery = WasPredicateQueryFactory.MakePredicateQuery(searcher, clause.Field.ToLower(), clause.Predicate, false);
                    var wasWithPredicateQuery = new BooleanQuery
                    {
                        {wasQuery, Occur.MUST},
                        {wasPredicateQuery, Occur.MUST}
                    };
                    wasQuery = wasWithPredicateQuery;
                }
                outerWasQuery.Add(wasQuery, Occur.SHOULD);
            }

            return outerWasQuery;
        }

        private TermQuery CreateTermQuery(IWasClause clause, string value, string documentField)
        {
            return new TermQuery(new Term(clause.Field.ToLower() + '.' + documentField, value));
        }

        private static string EncodeProtocol(string changeItem)
        {
            return DocumentConstants.ChangeHistoryProtocol + (changeItem?.ToLower() ?? "");
        }

        private static string EncodeProtocolPreservingCase(string changeItem)
        {
            return DocumentConstants.ChangeHistoryProtocol + (changeItem ?? "");
        }
    }
}