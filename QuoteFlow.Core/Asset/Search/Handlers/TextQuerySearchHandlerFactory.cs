using System;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Handlers;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Permission;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Core.Jql;
using QuoteFlow.Core.Jql.Context;
using QuoteFlow.Core.Jql.Query;
using QuoteFlow.Core.Jql.Validator;
using TextQuerySearcher = QuoteFlow.Core.Asset.Search.Searchers.TextQuerySearcher;

namespace QuoteFlow.Core.Asset.Search.Handlers
{
    /// <summary>
    /// Creates a SearchHandler for text ~ searches.
    /// </summary>
    public class TextQuerySearchHandlerFactory : ISearchHandlerFactory
    {
        private readonly IClauseValidator clauseValidator;
        private readonly IClauseContextFactory clauseContextFactory;
        private readonly IJqlOperandResolver operandResolver;
        private readonly IClauseQueryFactory clauseQueryFactory;
        private readonly IClauseInformation clauseInfo;

        public TextQuerySearchHandlerFactory(AllTextValidator clauseValidator,
            AllTextClauseContextFactory clauseContextFactory, ISearchHandlerManager searchHandlerManager, 
            IJqlOperandResolver operandResolver)
        {
            if (clauseContextFactory == null)
            {
                throw new ArgumentNullException(nameof(clauseContextFactory));
            }

            if (clauseValidator == null)
            {
                throw new ArgumentNullException(nameof(clauseValidator));
            }

            this.operandResolver = operandResolver;
            this.clauseContextFactory = clauseContextFactory;
            this.clauseQueryFactory = new AllTextClauseQueryFactory(searchHandlerManager);
            this.clauseInfo = SystemSearchConstants.ForAllText();
            this.clauseValidator = clauseValidator;
        }

        SearchHandler ISearchHandlerFactory.CreateHandler(ISearchableField field)
        {
            return CreateHandler();
        }

        public virtual SearchHandler CreateHandler()
        {
            var searcher = new TextQuerySearcher(operandResolver);
            var clauseHandler = new ClauseHandler(clauseInfo, clauseQueryFactory, clauseValidator, ClausePermissionHandler.NoopClausePermissionHandler,  clauseContextFactory);
            var searcherRegistration = new SearchHandler.SearcherRegistration(searcher, clauseHandler);
            var relatedIndexers = searcher.SearchInformation.RelatedIndexers;
            return new SearchHandler(relatedIndexers, searcherRegistration);
        }
    }
}