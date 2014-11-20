using System;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Assets.Search.Managers;
using QuoteFlow.Models.Assets.Search.Searchers;
using QuoteFlow.Models.Search.Jql.Clauses;
using QuoteFlow.Models.Search.Jql.Context;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Validator;
using QuoteFlow.Models.Search.Jql.Values;

namespace QuoteFlow.Models.Assets.Search.Handlers
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
                throw new ArgumentNullException("clauseContextFactory");
            }

            if (clauseValidator == null)
            {
                throw new ArgumentNullException("clauseValidator");
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
            var clauseHandler = new ClauseHandler(clauseInfo, clauseQueryFactory, clauseValidator, clauseContextFactory);
            var searcherRegistration = new SearchHandler.SearcherRegistration(searcher, clauseHandler);
            var relatedIndexers = searcher.SearchInformation.RelatedIndexers;
            return new SearchHandler(relatedIndexers, searcherRegistration);
        }
    }
}