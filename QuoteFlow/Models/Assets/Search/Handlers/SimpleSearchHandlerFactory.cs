using System;
using Ninject;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Search.Searchers;
using QuoteFlow.Models.Search.Jql.Clauses;
using QuoteFlow.Models.Search.Jql.Context;
using QuoteFlow.Models.Search.Jql.Permission;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Validator;
using QuoteFlow.Models.Search.Jql.Values;

namespace QuoteFlow.Models.Assets.Search.Handlers
{
    public abstract class SimpleSearchHandlerFactory : ISearchHandlerFactory
    {
        private readonly Type searcherClass;
        private readonly IClauseInformation clauseInformation;
        private readonly IClauseQueryFactory queryFactory;
        private readonly IClauseContextFactory _clauseContextFactory;
        private readonly IClauseValidator clauseValidator;
        private readonly FieldClausePermissionChecker.IFactory clausePermissionFactory;
        private readonly IClauseSanitizer sanitizer;
        private readonly IClauseValuesGenerator clauseValuesGenerator;

        /// <summary>
        /// Creates a new factory.
        /// </summary>
        /// <param name="information">Contains the string information (clause names, index field, field id) associated with this clause handler.</param>
        /// <param name="searcherClass">The class of the searcher to create.</param>
        /// <param name="queryFactory">The query factory place in the handler.</param>
        /// <param name="clauseValidator">The validator to place in the handler.</param>
        /// <param name="clausePermissionFactory">Used to create the ClausePermissionHandler. Use this to mainly prevent a circular dependency.</param>
        /// <param name="clauseContextFactory">The factory to place in the handler.</param>
        /// <param name="clauseValuesGenerator">Generates the possible values for a clause.</param>
        /// <param name="sanitizer">The sanitizer to place in the handler. If you want to use <see cref="NoOpClauseSanitizer"/>, use the other constructor.</param>
        public SimpleSearchHandlerFactory(IClauseInformation information, Type searcherClass, 
            IClauseQueryFactory queryFactory, IClauseValidator clauseValidator, 
            FieldClausePermissionChecker.IFactory clausePermissionFactory, IClauseContextFactory clauseContextFactory, 
            IClauseValuesGenerator clauseValuesGenerator, IClauseSanitizer sanitizer = null)
        {
            this._clauseContextFactory = clauseContextFactory;
            this.queryFactory = queryFactory;
            this.clauseValidator = clauseValidator;
            this.clausePermissionFactory = clausePermissionFactory;
            this.clauseInformation = information;
            this.searcherClass = searcherClass;
            this.clauseValuesGenerator = clauseValuesGenerator;
            this.sanitizer = sanitizer;
        }

        public SearchHandler CreateHandler(ISearchableField field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }

            IAssetSearcher<ISearchableField> searcher = CreateSearchableField(searcherClass, field);
            IClauseHandler clauseHandler;
            if (clauseValuesGenerator == null)
            {
                clauseHandler = new ClauseHandler(clauseInformation, queryFactory, clauseValidator, _clauseContextFactory);
            }
            else
            {
                clauseHandler = new ValuesGeneratingClauseHandler(clauseInformation, queryFactory, clauseValidator, _clauseContextFactory, clauseValuesGenerator);
            }
            
            var registration = new SearchHandler.ClauseRegistration(clauseHandler);
            var searcherRegistration = new SearchHandler.SearcherRegistration(searcher, registration);
            return new SearchHandler(searcher.SearchInformation.RelatedIndexers, searcherRegistration);
        }

        /// <summary>
        /// Method that creates and initialises a searcher of the passed class.
        /// </summary>
        /// <param name="clazz">The searcher to create.</param>
        /// <param name="field">The field the searcher is being created for.</param>
        /// <returns>The new initialized searcher.</returns>
        private IAssetSearcher<ISearchableField> CreateSearchableField(Type clazz, ISearchableField field)
        {
            try
            {
                var searcher = (IAssetSearcher<ISearchableField>)Container.Kernel.Get(clazz);
                searcher.Init(field);
                return searcher;
            }
            catch (Exception e)
            {   
                throw e;
            }
        }

        private IClausePermissionHandler CreateClausePermissionHandler(IField field)
        {
            if (sanitizer == null)
            {
                return new ClausePermissionHandler(clausePermissionFactory.CreatePermissionChecker(field));
            }
            return new ClausePermissionHandler(clausePermissionFactory.CreatePermissionChecker(field), sanitizer);
        }
    }
}