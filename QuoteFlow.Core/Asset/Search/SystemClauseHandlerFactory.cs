using System;
using System.Collections.Generic;
using Ninject;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Permission;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Core.DependencyResolution;
using QuoteFlow.Core.Jql.Context;
using QuoteFlow.Core.Jql.Permission;
using QuoteFlow.Core.Jql.Validator;

namespace QuoteFlow.Core.Asset.Search
{
    public class SystemClauseHandlerFactory : ISystemClauseHandlerFactory
    {
        private readonly FieldClausePermissionChecker.IFactory _fieldClausePermissionHandlerFactory;
        private readonly SearchHandlerBuilderFactory _searchHandlers;

        private readonly IJqlOperandResolver _jqlOperandResolver;
        private readonly IJqlAssetSupport _jqlAssetSupport;

        public SystemClauseHandlerFactory(FieldClausePermissionChecker.IFactory fieldClausePermissionHandlerFactory, SearchHandlerBuilderFactory searchHandlers, IJqlOperandResolver jqlOperandResolver, IJqlAssetSupport jqlAssetSupport)
        {
            _fieldClausePermissionHandlerFactory = fieldClausePermissionHandlerFactory;
            _searchHandlers = searchHandlers;
            _jqlOperandResolver = jqlOperandResolver;
            _jqlAssetSupport = jqlAssetSupport;
        }

        public IEnumerable<SearchHandler> GetSystemClauseSearchHandlers()
        {
            // TODO look into lazy instantiation of this
            var systemClauseSearchHandlers = new List<SearchHandler>
            {
//                CreateSavedFilterSearchHandler(),
                CreateAssetIdSearchHandler(),
//                createIssueParentSearchHandler(),
//                createCurrentEstimateSearchHandler(),
//                createOriginalEstimateSearchHandler(),
//                createTimeSpentSearchHandler(),
//                createSecurityLevelSearchHandler(),
//                createVotesSearchHandler(),
//                createVoterSearchHandler(),
//                createWatchesSearchHandler(),
//                createWatcherSearchHandler(),
//                createProjectCategoryHandler(),
//                createSubTaskSearchHandler(),
//                createProgressSearchHandler(),
//                createLastViewedHandler(),
//                createAttachmentsSearchHandler(),
//                createIssuePropertySearchHandler(),
//                createStatusCategorySearchHandler()
            };

            return systemClauseSearchHandlers;
        }

        private SearchHandler CreateAssetIdSearchHandler()
        {
            var assetIdClauseContextFactory = Container.Kernel.TryGet<AssetIdClauseContextFactory.Factory>();

            return _searchHandlers
                .Builder(SystemSearchConstants.ForAssetId())
                .SetClauseQueryFactoryType<AssetIdClauseQueryFactory>()
                .SetClauseValidatorType<AssetIdValidator>()
                .SetContextFactory(DecorateWithMultiContextFactory(assetIdClauseContextFactory.Create(SystemSearchConstants.ForAssetId().SupportedOperators)))
                .SetPermissionHandler(new ClausePermissionHandler(new AssetClauseValueSanitizer(_jqlOperandResolver, _jqlAssetSupport)))
                .BuildWithValuesGenerator(null);
        }

        private IClausePermissionHandler CreateClausePermissionHandler(string fieldId)
        {
            return new ClausePermissionHandler(_fieldClausePermissionHandlerFactory.CreatePermissionChecker(fieldId));
        }

        private static IClauseContextFactory DecorateWithMultiContextFactory(IClauseContextFactory factory)
        {
            var multiFactory = Container.Kernel.TryGet<MultiClauseDecoratorContextFactory.Factory>();
            return multiFactory.Create(factory);
        }

        private static IClauseContextFactory DecorateWithValidatingContextFactory(IClauseContextFactory factory)
        {
            var operatorUsageValidator = Container.Kernel.TryGet<IOperatorUsageValidator>();
            return new ValidatingDecoratorContextFactory(operatorUsageValidator, factory);
        }
    }
}