using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Mail;
using System.Security.Principal;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using AnglicanGeek.MarkdownMailer;
using Elmah;
using Lucene.Net.Store;
using Microsoft.WindowsAzure.ServiceRuntime;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using QuoteFlow.Api.Asset.CustomFields.Searchers.Transformer;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Asset.Search.Util;
using QuoteFlow.Api.Configuration;
using QuoteFlow.Api.Infrastructure.Lucene;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Parser;
using QuoteFlow.Api.Jql.Permission;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Operand.Registry;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset.CustomFields.Searchers.Transformer;
using QuoteFlow.Core.Asset.Fields;
using QuoteFlow.Core.Asset.Search;
using QuoteFlow.Core.Asset.Search.Handlers;
using QuoteFlow.Core.Asset.Search.Managers;
using QuoteFlow.Core.Asset.Search.Searchers;
using QuoteFlow.Core.Asset.Search.Util;
using QuoteFlow.Core.Auditing;
using QuoteFlow.Core.Configuration;
using QuoteFlow.Core.Jql.Context;
using QuoteFlow.Core.Jql.Operand;
using QuoteFlow.Core.Jql.Operand.Registry;
using QuoteFlow.Core.Jql.Parser;
using QuoteFlow.Core.Jql.Permission;
using QuoteFlow.Core.Jql.Query;
using QuoteFlow.Core.Jql.Resolver;
using QuoteFlow.Core.Jql.Util;
using QuoteFlow.Core.Jql.Validator;
using QuoteFlow.Core.Lucene;
using QuoteFlow.Core.Services;
using QuoteFlow.Infrastructure;
using QuoteFlow.Services;
using MessageService = QuoteFlow.Services.MessageService;

namespace QuoteFlow
{
    public class ContainerBindings : NinjectModule
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1502:CyclomaticComplexity", Justification = "This code is more maintainable in the same function.")]
        public override void Load()
        {
            var configuration = new ConfigurationService();
            Bind<ConfigurationService>().ToMethod(context => configuration);
            Bind<IAppConfiguration>().ToMethod(context => configuration.Current);
            Bind<IConfigurationSource>().ToMethod(context => configuration);

            Bind<Directory>()
                .ToMethod(_ => LuceneCommon.GetDirectory(configuration.Current.LuceneIndexLocation))
                .InSingletonScope();

            ConfigureSearch();

            if (!String.IsNullOrEmpty(configuration.Current.AzureStorageConnectionString))
            {
                Bind<ErrorLog>()
                    .ToMethod(_ => new TableErrorLog(configuration.Current.AzureStorageConnectionString))
                    .InSingletonScope();
            }
            else
            {
                Bind<ErrorLog>()
                    .ToMethod(_ => new SqlErrorLog(configuration.Current.SqlConnectionString))
                    .InSingletonScope();
            }

            Bind<ICacheService>()
                .To<HttpContextCacheService>()
                .InRequestScope();

            Bind<IContentService>()
                .To<ContentService>()
                .InSingletonScope();

            Bind<IAssetService>()
                .To<AssetService>()
                .InRequestScope();

            Bind<IAssetSearchService>()
                .To<AssetSearchService>()
                .InRequestScope();

            Bind<IAssetSearcherManager>().To<AssetSearcherManager>().InRequestScope();

            Bind<IAssetVarService>()
                .To<AssetVarService>()
                .InRequestScope();

            Bind<ICatalogImportService>().To<CatalogImportService>().InRequestScope();

            Bind<ICatalogImportSummaryRecordsService>()
                .To<CatalogImportSummaryRecordsService>()
                .InRequestScope();

            Bind<ICatalogService>()
                .To<CatalogService>()
                .InRequestScope();

            Bind<IContactService>()
                .To<ContactService>()
                .InRequestScope();

            Bind<IManufacturerService>()
                .To<ManufacturerService>()
                .InRequestScope();

            Bind<IManufacturerLogoService>()
                .To<ManufacturerLogoService>()
                .InRequestScope();

            Bind<IOrganizationService>()
                .To<OrganizationService>()
                .InRequestScope();

            Bind<IQuoteService>()
                .To<QuoteService>()
                .InRequestScope();

            Bind<ISearchHandlerManager>().To<SearchHandlerManager>().InRequestScope();

            Bind<ISearchService>()
                .To<SearchService>()
                .InRequestScope();

            Bind<IUserService>()
                .To<UserService>()
                .InRequestScope();

            Bind<IControllerFactory>()
                .To<QuoteFlowControllerFactory>()
                .InRequestScope();

            var mailSenderThunk = new Lazy<IMailSender>(
                () =>
                {
                    var settings = Kernel.Get<ConfigurationService>();
                    if (settings.Current.SmtpUri != null && settings.Current.SmtpUri.IsAbsoluteUri)
                    {
                        var smtpUri = new SmtpUri(settings.Current.SmtpUri);

                        var mailSenderConfiguration = new MailSenderConfiguration
                        {
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            Host = smtpUri.Host,
                            Port = smtpUri.Port,
                            EnableSsl = smtpUri.Secure
                        };

                        if (!String.IsNullOrWhiteSpace(smtpUri.UserName))
                        {
                            mailSenderConfiguration.UseDefaultCredentials = false;
                            mailSenderConfiguration.Credentials = new NetworkCredential(
                                smtpUri.UserName,
                                smtpUri.Password);
                        }

                        return new MailSender(mailSenderConfiguration);
                    }
                    else
                    {
                        var mailSenderConfiguration = new MailSenderConfiguration
                        {
                            DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                            PickupDirectoryLocation = HostingEnvironment.MapPath("~/App_Data/Mail")
                        };

                        return new MailSender(mailSenderConfiguration);
                    }
                });

            Bind<IMailSender>()
                .ToMethod(context => mailSenderThunk.Value);

            Bind<IMessageService>()
                .To<MessageService>();

            Bind<IPrincipal>().ToMethod(context => HttpContext.Current.User);

            switch (configuration.Current.StorageType)
            {
                case StorageType.FileSystem:
                case StorageType.NotSpecified:
                    ConfigureForLocalFileSystem();
                    break;
                case StorageType.AzureStorage:
                    ConfigureForAzureStorage(configuration);
                    break;
            }

            Bind<IFileSystemService>()
                .To<FileSystemService>()
                .InSingletonScope();

            Bind<IUploadFileService>()
                .To<UploadFileService>();

            #region Asset Fields

            Bind<FieldClausePermissionChecker.IFactory>().To<FieldClausePermissionChecker.Factory>().InRequestScope();
            Bind<ICustomFieldInputHelper>().To<CustomFieldInputHelper>().InRequestScope();
            Bind<IFieldFlagOperandRegistry>().To<FieldFlagOperandRegistry>().InRequestScope();
            Bind<IFieldManager>().To<FieldManager>().InRequestScope();

            #endregion

            #region Jql Support

            Bind<IJqlAssetIdSupport>().To<JqlAssetIdSupport>().InRequestScope();
            Bind<IJqlDateSupport>().To<JqlDateSupport>().InRequestScope();
            RequestScopeExtensionMethod.InRequestScope<LazyResettableJqlFunctionHandlerRegistry>(Bind<IJqlFunctionHandlerRegistry>().To<LazyResettableJqlFunctionHandlerRegistry>());
            Bind<IJqlOperandResolver>().To<JqlOperandResolver>().InRequestScope();
            Bind<IJqlQueryParser>().To<JqlQueryParser>().InRequestScope();
            Bind<IJqlStringSupport>().To<JqlStringSupport>().InRequestScope();
            Bind<IOperatorUsageValidator>().To<OperatorUsageValidator>().InRequestScope();
            Bind<IQueryCache>().To<QueryCache>().InRequestScope();
            Bind<IQueryRegistry>().To<QueryRegistry>().InRequestScope();
            Bind<ISystemClauseHandlerFactory>().To<SystemClauseHandlerFactory>().InRequestScope();
            Bind<IValidatorRegistry>().To<ValidatorRegistry>().InRequestScope();
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

            #endregion
        }

        private void ConfigureSearch()
        {
            Bind<IAssetIndexManager>().To<AssetIndexManager>().InRequestScope();
            Bind<ILuceneQueryBuilder>().To<LuceneQueryBuilder>().InRequestScope();
            Bind<ILuceneQueryModifier>().To<LuceneQueryModifier>().InRequestScope();
            Bind<ISearchProvider>().To<LuceneSearchService>().InRequestScope();
            Bind<ISearchProviderFactory>().To<SearchProviderFactory>().InRequestScope();
            Bind<ISearchSortUtil>().To<SearchSortUtil>().InRequestScope();
            Bind<IIndexingService>().To<LuceneIndexingService>().InRequestScope();

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

        private void ConfigureForLocalFileSystem()
        {
            Bind<IFileStorageService>()
                .To<FileSystemFileStorageService>()
                .InSingletonScope();

            // Ninject is doing some weird things with constructor selection without these.
            // Anyone requesting an IReportService or IStatisticsService should be prepared
            // to receive null anyway.
//            Bind<IReportService>().ToConstant(NullReportService.Instance);
//            Bind<IStatisticsService>().ToConstant(NullStatisticsService.Instance);
            Bind<AuditingService>().ToConstant(AuditingService.None);
        }

        private void ConfigureForAzureStorage(ConfigurationService configuration)
        {
            string instanceId;
            try
            {
                instanceId = RoleEnvironment.CurrentRoleInstance.Id;
            }
            catch (Exception)
            {
                instanceId = Environment.MachineName;
            }

            var localIP = AuditActor.GetLocalIP().Result;

            Bind<AuditingService>()
                .ToMethod(_ => new CloudAuditingService(
                    instanceId, localIP, configuration.Current.AzureStorageConnectionString, CloudAuditingService.AspNetActorThunk))
                .InSingletonScope();
        }
    }
}
