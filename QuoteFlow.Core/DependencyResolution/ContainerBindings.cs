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
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using QuoteFlow.Api.Configuration;
using QuoteFlow.Api.Configuration.Lucene;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Auditing;
using QuoteFlow.Core.Configuration;
using QuoteFlow.Core.Configuration.Lucene;
using QuoteFlow.Core.Infrastructure.Mvc;
using QuoteFlow.Core.Services;

namespace QuoteFlow.Core.DependencyResolution
{
    public class ContainerBindings : NinjectModule
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1502:CyclomaticComplexity", Justification = "This code is more maintainable in the same function.")]
        public override void Load()
        {
            var configuration = new ConfigurationService();
            Bind<ConfigurationService>().ToMethod(context => configuration);
            Bind<IAppConfiguration>().ToMethod(context => configuration.Current);
            Bind<IAssetTableServiceConfiguration>().ToMethod(context => configuration.AssetTableConfig);
            Bind<IIndexingConfiguration>().ToMethod(context => configuration.IndexingConfiguration);
            Bind<IConfigurationSource>().ToMethod(context => configuration);

            Bind<Directory>()
                .ToMethod(_ => IndexPathManager.GetDirectory(configuration.Current.LuceneIndexLocation))
                .InSingletonScope();

            Bind<ErrorLog>()
                .ToMethod(_ => new SqlErrorLog(configuration.Current.SqlConnectionString))
                .InSingletonScope();

            Bind<ICacheService>().To<HttpContextCacheService>().InRequestScope();
            Bind<IContentService>().To<ContentService>().InSingletonScope();

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
        }

        private void ConfigureForLocalFileSystem()
        {
            Bind<IFileStorageService>()
                .To<FileSystemFileStorageService>()
                .InSingletonScope();

            // Ninject is doing some weird things with constructor selection without these.
            // Anyone requesting an IReportService or IStatisticsService should be prepared
            // to receive null anyway.
            Bind<AuditingService>().ToConstant(AuditingService.None);
        }

        private void ConfigureForAzureStorage(ConfigurationService configuration)
        {
//            string instanceId;
//            try
//            {
//                instanceId = RoleEnvironment.CurrentRoleInstance.Id;
//            }
//            catch (Exception)
//            {
//                instanceId = Environment.MachineName;
//            }
//
//            var localIP = AuditActor.GetLocalIP().Result;
//
//            Bind<AuditingService>()
//                .ToMethod(_ => new CloudAuditingService(
//                    instanceId, localIP, configuration.Current.AzureStorageConnectionString, CloudAuditingService.AspNetActorThunk))
//                .InSingletonScope();
        }
    }
}
