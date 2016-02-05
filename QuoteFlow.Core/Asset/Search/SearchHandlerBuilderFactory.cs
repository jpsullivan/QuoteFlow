using System;
using System.Collections;
using System.Collections.Generic;
using Ninject;
using QuoteFlow.Api.Asset.Index.Indexers;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Permission;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Api.Jql.Values;
using QuoteFlow.Core.DependencyResolution;
using QuoteFlow.Core.Jql;
using QuoteFlow.Core.Jql.Context;

namespace QuoteFlow.Core.Asset.Search
{
    /// <summary>
    /// Component that can produce <see cref="SearchHandler"/> builders.
    /// </summary>
    public class SearchHandlerBuilderFactory
    {
        public SearchHandlerBuilder Builder(IClauseInformation clauseInfo)
        {
            return new SearchHandlerBuilder(clauseInfo);
        }
    }

    /// <summary>
    /// Builder that can produce <see cref="SearchHandler"/> instances.
    /// </summary>
    public class SearchHandlerBuilder
    {
        private readonly IClauseInformation _clauseInformation;
        private List<IFieldIndexer> _fieldIndexers = new List<IFieldIndexer>();

        private IClauseContextFactory _clauseContextFactory = Container.Kernel.TryGet<SimpleClauseContextFactory>();
        private IClauseQueryFactory _clauseQueryFactory;
        private IClauseValidator _clauseValidator;
        private IClausePermissionHandler _permissionHandler;

        public SearchHandlerBuilder(IClauseInformation clauseInformation)
        {
            _clauseInformation = clauseInformation;
        }

        public SearchHandlerBuilder SetClauseQueryFactoryType<T>() where T : IClauseQueryFactory
        {
            _clauseQueryFactory = Container.Kernel.TryGet<T>();
            return this;
        }

        public SearchHandlerBuilder SetClauseValidatorType<T>() where T : IClauseValidator
        {
            _clauseValidator = Container.Kernel.TryGet<T>();
            return this;
        }

        public SearchHandlerBuilder SetContextFactory(IClauseContextFactory classContextFactoryType)
        {
            _clauseContextFactory = classContextFactoryType;
            return this;
        }

        public SearchHandlerBuilder SetPermissionHandler(IClausePermissionHandler permissionHandler)
        {
            _permissionHandler = permissionHandler;
            return this;
        }

        public SearchHandlerBuilder SetPermissionChecker(IClausePermissionChecker permissionChecker)
        {
            _permissionHandler = new ClausePermissionHandler(permissionChecker);
            return this;
        }

        public SearchHandlerBuilder AddIndexer(IFieldIndexer indexer)
        {
            _fieldIndexers.Add(indexer);
            return this;
        }

        public SearchHandler BuildWithValuesGenerator(IClauseValuesGenerator generator)
        {
            return
                WithClauseHandler(new ValuesGeneratingClauseHandler(_clauseInformation, _clauseQueryFactory,
                    _clauseValidator, _clauseContextFactory, _permissionHandler, generator));
        }

        public SearchHandler Build()
        {

            return WithClauseHandler(new ClauseHandler(_clauseInformation, _clauseQueryFactory,
                _clauseValidator, _permissionHandler, _clauseContextFactory));
        }

        private SearchHandler WithClauseHandler(IClauseHandler clauseHandler)
        {
            // check for any weird state
            if (_clauseContextFactory == null) throw new InvalidOperationException();
            if (_clauseValidator == null) throw new InvalidOperationException();
            if (_permissionHandler == null) throw new InvalidOperationException();
            if (_clauseQueryFactory == null) throw new InvalidOperationException();

            var clauseRegistration = new SearchHandler.ClauseRegistration(clauseHandler);
            return new SearchHandler(_fieldIndexers, null, new List<SearchHandler.ClauseRegistration> {clauseRegistration});
        }
    }
}