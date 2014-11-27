using System;
using System.Net;
using System.Net.Mail;
using System.Security.Principal;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using AnglicanGeek.MarkdownMailer;
using Elmah;
using Microsoft.WindowsAzure.ServiceRuntime;
using Ninject;
using Ninject.Web.Common;
using Ninject.Modules;
using QuoteFlow.Auditing;
using QuoteFlow.Configuration;
using QuoteFlow.Infrastructure;
using QuoteFlow.Infrastructure.Lucene;
using QuoteFlow.Models;
using QuoteFlow.Models.Assets.CustomFields.Searchers.Transformer;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Search;
using QuoteFlow.Models.Assets.Search.Handlers;
using QuoteFlow.Models.Assets.Search.Managers;
using QuoteFlow.Models.Assets.Search.Searchers;
using QuoteFlow.Models.Assets.Search.Searchers.Transformer;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Parser;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Operand.Registry;
using QuoteFlow.Models.Search.Jql.Resolver;
using QuoteFlow.Models.Search.Jql.Util;
using QuoteFlow.Models.Search.Jql.Validator;
using QuoteFlow.Services;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow
{
    public class ContainerBindings : NinjectModule
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:CyclomaticComplexity", Justification = "This code is more maintainable in the same function.")]
        public override void Load()
        {
            var configuration = new ConfigurationService();
            Bind<ConfigurationService>().ToMethod(context => configuration);
            Bind<IAppConfiguration>().ToMethod(context => configuration.Current);
            Bind<PoliteCaptcha.IConfigurationSource>().ToMethod(context => configuration);

            Bind<Lucene.Net.Store.Directory>()
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

            Bind<IFormsAuthenticationService>()
                .To<FormsAuthenticationService>()
                .InSingletonScope();

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

            Bind<ICustomFieldInputHelper>().To<CustomFieldInputHelper>().InRequestScope();
            Bind<IFieldFlagOperandRegistry>().To<FieldFlagOperandRegistry>().InRequestScope();
            Bind<IFieldManager>().To<FieldManager>().InRequestScope();

            #endregion

            #region Jql Support

            Bind<IJqlAssetIdSupport>().To<JqlAssetIdSupport>().InRequestScope();
            Bind<IJqlFunctionHandlerRegistry>().To<LazyResettableJqlFunctionHandlerRegistry>().InRequestScope();
            Bind<IJqlOperandResolver>().To<JqlOperandResolver>().InRequestScope();
            Bind<IJqlQueryParser>().To<JqlQueryParser>().InRequestScope();
            Bind<IJqlStringSupport>().To<JqlStringSupport>().InRequestScope();
            Bind<IOperatorUsageValidator>().To<OperatorUsageValidator>().InRequestScope();
            Bind<IQueryCache>().To<QueryCache>().InRequestScope();
            Bind<IQueryRegistry>().To<QueryRegistry>().InRequestScope();
            Bind<ISystemClauseHandlerFactory>().To<SystemClauseHandlerFactory>().InRequestScope();

            #endregion

            #region Jql Name Resolvers

            Bind<INameResolver<Catalog>>().To<CatalogResolver>().InRequestScope();

            #endregion

            #region Jql IndexInfo Resolvers

            Bind<IIndexInfoResolver<Catalog>>().To<CatalogIndexInfoResolver>().InRequestScope();

            #endregion

            #region System Fields

            Bind<CatalogSystemField>().ToSelf().InRequestScope();

            #endregion

            #region Jql Searchers

            // catalog searching
            Bind<CatalogClauseQueryFactory>().ToSelf().InRequestScope();
            Bind<CatalogValidator>().ToSelf().InRequestScope();
            Bind<CatalogSearchHandlerFactory>().ToSelf().InRequestScope();
            Bind<CatalogResolver>().ToSelf().InRequestScope();

            // summary (asset name) searching
            Bind<SummaryValidator>().ToSelf().InRequestScope();
            Bind<SummaryClauseQueryFactory>().ToSelf().InRequestScope();
            Bind<SummarySearchHandlerFactory>().ToSelf().InRequestScope();

            #endregion
        }

        private void ConfigureSearch()
        {
            Bind<CatalogSearcher>().ToSelf().InRequestScope();
            Bind<ILuceneQueryBuilder>().To<LuceneQueryBuilder>().InRequestScope();
            Bind<ILuceneQueryModifier>().To<LuceneQueryModifier>().InRequestScope();
            Bind<ISearchProvider>().To<LuceneSearchService>().InRequestScope();
            Bind<IIndexingService>().To<LuceneIndexingService>().InRequestScope();
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
            Bind<ICloudBlobClient>()
                .ToMethod(
                    _ => new CloudBlobClientWrapper(configuration.Current.AzureStorageConnectionString))
                .InSingletonScope();
            Bind<IFileStorageService>()
                .To<CloudBlobFileStorageService>()
                .InSingletonScope();

            // when running on Windows Azure, pull the statistics from the warehouse via storage
//            Bind<IReportService>()
//                .ToMethod(context => new CloudReportService(configuration.Current.AzureStorageConnectionString))
//                .InSingletonScope();
//            Bind<IStatisticsService>()
//                .To<JsonStatisticsService>()
//                .InSingletonScope();

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
