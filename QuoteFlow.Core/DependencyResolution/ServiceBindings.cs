using Ninject.Modules;
using Ninject.Web.Common;
using QuoteFlow.Api.Asset;
using QuoteFlow.Api.Asset.CustomFields.Searchers.Transformer;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Nav;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Asset.Search.Util;
using QuoteFlow.Api.Auditing;
using QuoteFlow.Api.Configuration.Lucene;
using QuoteFlow.Api.Infrastructure.Lucene;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Parser;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Operand.Registry;
using QuoteFlow.Api.Jql.Query.Order;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Api.Jql.Values;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Api.UserTracking;
using QuoteFlow.Core.Asset;
using QuoteFlow.Core.Asset.CustomFields.Searchers.Transformer;
using QuoteFlow.Core.Asset.Fields;
using QuoteFlow.Core.Asset.Index;
using QuoteFlow.Core.Asset.Index.Managers;
using QuoteFlow.Core.Asset.Nav;
using QuoteFlow.Core.Asset.Search;
using QuoteFlow.Core.Asset.Search.Handlers;
using QuoteFlow.Core.Asset.Search.Managers;
using QuoteFlow.Core.Asset.Search.Searchers;
using QuoteFlow.Core.Asset.Search.Util;
using QuoteFlow.Core.Auditing;
using QuoteFlow.Core.Configuration.Lucene;
using QuoteFlow.Core.Index;
using QuoteFlow.Core.Jql.Builder;
using QuoteFlow.Core.Jql.Context;
using QuoteFlow.Core.Jql.Operand;
using QuoteFlow.Core.Jql.Operand.Registry;
using QuoteFlow.Core.Jql.Parser;
using QuoteFlow.Core.Jql.Permission;
using QuoteFlow.Core.Jql.Query;
using QuoteFlow.Core.Jql.Query.Order;
using QuoteFlow.Core.Jql.Resolver;
using QuoteFlow.Core.Jql.Util;
using QuoteFlow.Core.Jql.Validator;
using QuoteFlow.Core.Jql.Values;
using QuoteFlow.Core.Lucene.Index;
using QuoteFlow.Core.Services;
using QuoteFlow.Core.UserTracking;
using QuoteFlow.Core.Util.Index;
using QuoteFlow.Services;

namespace QuoteFlow.Core.DependencyResolution
{
    public class ServiceBindings : NinjectModule
    {
        public override void Load()
        {   
            Bind<IAssetFactory>().To<AssetFactory>().InRequestScope();
            Bind<IAssetService>().To<AssetService>().InRequestScope();
            Bind<IAssetSearchService>().To<AssetSearchService>().InRequestScope();
            Bind<IAssetSearcherManager>().To<AssetSearcherManager>().InRequestScope();
            Bind<IAssetVarService>().To<AssetVarService>().InRequestScope();
            Bind<IAuditService>().To<AuditService>().InRequestScope();
            Bind<ICatalogImportService>().To<CatalogImportService>().InRequestScope();
            Bind<ICatalogImportSummaryRecordsService>().To<CatalogImportSummaryRecordsService>().InRequestScope();
            Bind<ICatalogService>().To<CatalogService>().InRequestScope();
            Bind<IContactService>().To<ContactService>().InRequestScope();
            Bind<IManufacturerService>().To<ManufacturerService>().InRequestScope();
            Bind<IManufacturerLogoService>().To<ManufacturerLogoService>().InRequestScope();
            Bind<IOrganizationService>().To<OrganizationService>().InRequestScope();
            Bind<IQuoteLineItemService>().To<QuoteLineItemService>().InRequestScope();
            Bind<IQuoteService>().To<QuoteService>().InRequestScope();
            Bind<IQuoteStatusService>().To<QuoteStatusService>().InRequestScope();
            Bind<ISearchService>().To<SearchService>().InRequestScope();
            Bind<IUserService>().To<UserService>().InRequestScope();
            Bind<IUserTrackingService>().To<UserTrackingService>().InRequestScope();

            Bind<IFileSystemService>().To<FileSystemService>().InSingletonScope();
            Bind<IUploadFileService>().To<UploadFileService>();

            #region Asset Fields

            Bind<IAssetTableCreatorFactory>().To<AssetTableCreatorFactory>().InRequestScope();
            Bind<IAssetTableService>().To<AssetTableService>().InRequestScope();
            Bind<FieldClausePermissionChecker.IFactory>().To<FieldClausePermissionChecker.Factory>().InRequestScope();
            Bind<ICustomFieldInputHelper>().To<CustomFieldInputHelper>().InRequestScope();
            Bind<IFieldFlagOperandRegistry>().To<FieldFlagOperandRegistry>().InRequestScope();
            Bind<IFieldManager>().To<FieldManager>().InRequestScope();

            #endregion

            #region Jql Support

            Bind<IJqlAssetIdSupport>().To<JqlAssetIdSupport>().InRequestScope();
            Bind<IJqlClauseBuilderFactory>().To<JqlClauseBuilderFactory>().InRequestScope();
            Bind<IJqlDateSupport>().To<JqlDateSupport>().InRequestScope();
            Bind<IJqlFunctionHandlerRegistry>().To<LazyResettableJqlFunctionHandlerRegistry>().InRequestScope();
            Bind<IJqlOperandResolver>().To<JqlOperandResolver>().InRequestScope();
            Bind<IJqlQueryParser>().To<JqlQueryParser>().InRequestScope();
            Bind<IJqlStringSupport>().To<JqlStringSupport>().InRequestScope();
            Bind<IOperatorUsageValidator>().To<OperatorUsageValidator>().InRequestScope();
            Bind<IQueryCache>().To<QueryCache>().InRequestScope();
            Bind<IQueryRegistry>().To<QueryRegistry>().InRequestScope();
            Bind<ISystemClauseHandlerFactory>().To<SystemClauseHandlerFactory>().InRequestScope();
            Bind<IValidatorRegistry>().To<ValidatorRegistry>().InRequestScope();
            Bind<ISortJqlGenerator>().To<SortJqlGenerator>().InRequestScope();
            Bind<IOrderByUtil>().To<OrderByUtil>().InRequestScope();
            Bind<QueryContextConverter>().ToSelf().InRequestScope();
            Bind<ValidatorVisitor.ValidatorVisitorFactory>().ToSelf().InRequestScope();

            #endregion

            #region Jql Name Resolvers

            Bind<INameResolver<Catalog>>().To<CatalogResolver>().InRequestScope();
            Bind<INameResolver<Manufacturer>>().To<ManufacturerResolver>().InRequestScope();

            #endregion

            #region Jql IndexInfo Resolvers

            Bind<IIndexInfoResolver<Catalog>>().To<CatalogIndexInfoResolver>().InRequestScope();

            #endregion

            #region System Fields

            Bind<CatalogSystemField>().ToSelf().InRequestScope();
            Bind<SummarySystemField>().ToSelf().InRequestScope();

            #endregion

            #region Jql Searchers

            // catalog searching
            Bind<CatalogClauseQueryFactory>().ToSelf().InRequestScope();
            Bind<CatalogValidator>().ToSelf().InRequestScope();
            Bind<CatalogSearchHandlerFactory>().ToSelf().InRequestScope();
            Bind<CatalogResolver>().ToSelf().InRequestScope();
            Bind<CatalogSearcher>().ToSelf().InRequestScope();

            // manufacturer searching
            Bind<ManufacturerResolver>().ToSelf().InRequestScope();
            Bind<ManufacturerClauseQueryFactory>().ToSelf().InRequestScope();
            Bind<ManufacturerValidator>().ToSelf().InRequestScope();
            Bind<ManufacturerSearchHandlerFactory>().ToSelf().InRequestScope();
            Bind<ManufacturerSearcher>().ToSelf().InRequestScope();

            // summary (asset name) searching
            Bind<SummaryValidator>().ToSelf().InRequestScope();
            Bind<SummaryClauseQueryFactory>().ToSelf().InRequestScope();
            Bind<SummarySearchHandlerFactory>().ToSelf().InRequestScope();

            // creator searching
            Bind<CreatorSearcher>().ToSelf().InRequestScope();

            #endregion

            ConfigureSearch();
        }

        private void ConfigureSearch()
        {
            Bind<IAssetDocumentFactory>().To<AssetDocumentFactory>();
            Bind<IAssetIndexer>().To<AssetIndexer>().InRequestScope();
            Bind<IAssetIndexManager>().To<AssetIndexManager>().InRequestScope();
            Bind<IDocumentCreationStrategy>().To<DocumentCreationStrategy>().InRequestScope();
            Bind<IFieldIndexerManager>().To<FieldIndexerManager>().InRequestScope();
            Bind<IIndexDirectoryFactory>().To<IndexDirectoryFactory>().InRequestScope();
            Bind<IIndexLifecycleManager>().To<CompositeIndexLifecycleManager>().InRequestScope();
            Bind<IIndexManager>().To<DefaultIndexManager>().InRequestScope();
            Bind<IIndexPathManager>().To<IndexPathManager>().InRequestScope();
            Bind<IIndexWriterConfiguration>().To<IndexConfiguration.Default.IndexWriterConfiguration>().InRequestScope();
            Bind<ILuceneQueryBuilder>().To<LuceneQueryBuilder>().InRequestScope();
            Bind<ILuceneQueryModifier>().To<LuceneQueryModifier>().InRequestScope();
            Bind<ISearchContextHelper>().To<SearchContextHelper>().InRequestScope();
            Bind<ISearchExtractorRegistrationManager>().To<SearchExtractorRegistrationManager>().InRequestScope();
            Bind<ISearchHandlerManager>().To<SearchHandlerManager>().InRequestScope();
            Bind<ISearchProvider>().To<LuceneSearchService>().InRequestScope();
            Bind<ISearchProviderFactory>().To<SearchProviderFactory>().InRequestScope();
            Bind<ISearchSortUtil>().To<SearchSortUtil>().InRequestScope();

            #region Entity Document Builders

            Bind<IEntityDocumentBuilder<IAsset>>().To<AssetDocumentBuilder>();
            Bind<IEntityDocumentBuilder<AssetComment>>().To<CommentDocumentBuilder>();

            #endregion

            #region All-Text Searching

            Bind<AllTextClauseQueryFactory>().ToSelf().InRequestScope();
            Bind<AllTextValidator>().ToSelf().InRequestScope();
            Bind<AllTextClauseContextFactory>().ToSelf().InRequestScope();

            #endregion

            #region Change History Searching

            Bind<HistoryPredicateQueryFactory>().ToSelf().InRequestScope();
            Bind<EmptyWasClauseOperandHandler>().ToSelf().InRequestScope();
            Bind<WasClauseQueryFactory>().ToSelf().InRequestScope();
            Bind<WasClauseValidator>().ToSelf().InRequestScope();
            Bind<ChangedClauseQueryFactory>().ToSelf().InRequestScope();
            Bind<ChangedClauseValidator>().ToSelf().InRequestScope();
            Bind<ChangeHistoryFieldIdResolver>().ToSelf().InRequestScope();

            #endregion
        }
    }
}