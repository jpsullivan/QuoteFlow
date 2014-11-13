using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Index.Indexers;
using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Assets.Search.Managers;
using QuoteFlow.Models.Assets.Search.Searchers;
using QuoteFlow.Models.Search.Jql.Clauses;
using QuoteFlow.Models.Search.Jql.Context;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Permission;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Validator;
using QuoteFlow.Models.Search.Jql.Values;

namespace QuoteFlow.Models.Assets.Search.Handlers
{
    /// <summary>
    /// Creates a SearchHandler for text ~ searches.
    /// 
    /// @since v5.2
    /// </summary>
    public class TextQuerySearchHandlerFactory : ISearchHandlerFactory
    {
        private readonly IClauseValidator clauseValidator;
        private readonly IClauseContextFactory clauseContextFactory;
        private IJqlOperandResolver operandResolver;
        private readonly IClauseQueryFactory clauseQueryFactory;
        private readonly IClauseInformation clauseInfo;

        public TextQuerySearchHandlerFactory(AllTextValidator clauseValidator,
            AllTextClauseContextFactory clauseContextFactory, CustomFieldManager customFieldManager,
            SearchHandlerManager searchHandlerManager, JqlOperandResolver operandResolver)
        {
            if (clauseContextFactory == null)
            {
                throw new ArgumentNullException("clauseContextFactory");
            }

            if (clauseValidator == null)
            {
                throw new ArgumentNullException("clauseValidator");
            }

            this.operandResolver = operandResolver;
            this.clauseContextFactory = clauseContextFactory;
            this.clauseQueryFactory = new AllTextClauseQueryFactory(customFieldManager, searchHandlerManager);
            this.clauseInfo = SystemSearchConstants.ForAllText();
            this.clauseValidator = clauseValidator;
        }

        public override SearchHandler CreateHandler(ISearchableField field)
        {
            return CreateHandler();
        }

        public virtual SearchHandler CreateHandler()
        {
            var searcher = new TextQuerySearcher(operandResolver);
            var clauseHandler = new ClauseHandler(clauseInfo, clauseQueryFactory, clauseValidator, clauseContextFactory);
            var searcherRegistration = new SearchHandler.SearcherRegistration(searcher, clauseHandler);
            var relatedIndexers = searcher.SearchInformation.RelatedIndexers;
            return new SearchHandler(relatedIndexers, searcherRegistration);
        }
    }
}