using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QuoteFlow.Api.Asset.Nav;
using QuoteFlow.Api.Configuration;
using QuoteFlow.Api.Infrastructure.Helpers;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Api.Models.ViewModels.Quotes;
using QuoteFlow.Api.Services;
using QuoteFlow.Api.UserTracking;
using QuoteFlow.Infrastructure;
using QuoteFlow.Infrastructure.Attributes;
using QuoteFlow.Infrastructure.Extensions;

namespace QuoteFlow.Controllers
{
    [Authorize]
    public class QuoteController : AppController
    {
        #region DI

        public IAssetService AssetService { get; protected set; }
        public IAssetTableService AssetTableService { get; protected set; }
        public IAssetTableServiceConfiguration AssetTableServiceConfiguration { get; protected set; }
        public ICatalogService CatalogService { get; protected set; }
        public ICustomerService CustomerService { get; protected set; }
        public IJqlStringSupport JqlStringSupport { get; protected set; }
        public IManufacturerService ManufacturerService { get; protected set; }
        public IQuoteLineItemService QuoteLineItemService { get; protected set; }
        public IQuoteService QuoteService { get; protected set; }
        public IQuoteStatusService QuoteStatusService { get; protected set; }
        public ISearcherService SearcherService { get; protected set; }
        public IUserService UserService { get; protected set; }
        public IUserTrackingService UserTrackingService { get; protected set; }

        public QuoteController()
        {
        }

        public QuoteController(IAssetService assetService, IAssetTableService assetTableService, IAssetTableServiceConfiguration assetTableServiceConfiguration, ICatalogService catalogService, ICustomerService customerService, IJqlStringSupport jqlStringSupport, IManufacturerService manufacturerService, IQuoteLineItemService quoteLineItemService, IQuoteService quoteService, IQuoteStatusService quoteStatusService, ISearcherService searcherService, IUserService userService, IUserTrackingService userTrackingService)
        {
            AssetService = assetService;
            AssetTableService = assetTableService;
            AssetTableServiceConfiguration = assetTableServiceConfiguration;
            CatalogService = catalogService;
            CustomerService = customerService;
            JqlStringSupport = jqlStringSupport;
            ManufacturerService = manufacturerService;
            QuoteLineItemService = quoteLineItemService;
            QuoteService = quoteService;
            QuoteStatusService = quoteStatusService;
            SearcherService = searcherService;
            UserService = userService;
            UserTrackingService = userTrackingService;
        }

        #endregion

        [QuoteFlowRoute("quotes", Name = RouteNames.QuoteIndex)]
        public virtual ActionResult Index()
        {
            var quotes = QuoteService.GetQuotesFromOrganization(CurrentOrganization.Id);
            var model = new QuotesViewModel(quotes);
            return View(model);
        }

        [QuoteFlowRoute("quote/new")]
        public virtual ActionResult New()
        {
            var contacts = CustomerService.GetCustomersFromOrganization(CurrentOrganization.Id);
            var model = new NewQuoteModel
            {
                Customers = contacts
            };
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [QuoteFlowRoute("quote/create", HttpVerbs.Post)]
        public virtual ActionResult CreateQuote(NewQuoteModel model)
        {
            var currentUser = GetCurrentUser();

            // lazily set the organization to the first one that the user is assigned to.
            // eventually use whatever the users' current organization is once multi-tenancy
            // is implemented.
            model.Organization = currentUser.Organizations.First();

            if (!ModelState.IsValid)
            {
                return New();
            }

            // check to make sure that a quote with this name doesn't already exist
            if (QuoteService.ExistsWithinOrganization(model.QuoteName, CurrentOrganization.Id))
            {
                var errorMsg = $"Quote name already exists within the {model.Organization.OrganizationName} organization.";
                ModelState.AddModelError("Name", errorMsg);
                return View("New", model);
            }

            var newQuote = QuoteService.CreateQuote(model, currentUser.Id);
            return SafeRedirect(Url.Quote(newQuote.Id, newQuote.Name.UrlFriendly()));
        }

        [QuoteFlowRoute("quote/{id:INT}/{name}", Name = RouteNames.QuoteShow)]
        public virtual ActionResult Show(int id, string name)
        {
            var quote = QuoteService.GetQuote(id);
            if (quote == null)
            {
                return PageNotFound();
            }

            // track that this quote has been visited
            UserTrackingService.UpdateRecentLinks(GetCurrentUser().Id, PageType.Quote, quote.Id, quote.Name);

            var creator = UserService.GetUser(quote.CreatorId);
            var statuses = QuoteStatusService.GetStatuses(CurrentOrganization.Id);

            var model = new QuoteShowModel(quote, creator, statuses);
            return quote.Name.UrlFriendly() != name ? PageNotFound() : View(model);
        }

        [QuoteFlowRoute("quote/{id:INT}/{name}/line-items", Name = RouteNames.QuoteLineItems)]
        public virtual ActionResult LineItems(int id, string name, int? page)
        {
            var quote = QuoteService.GetQuote(id);
            if (quote == null)
            {
                return PageNotFound();
            }

            // track that this quote has been visited
            UserTrackingService.UpdateRecentLinks(GetCurrentUser().Id, PageType.Quote, quote.Id, quote.Name);

            const int perPage = 50;
            var currentPage = Math.Max(page ?? 1, 1);

            var creator = UserService.GetUser(quote.CreatorId);
            var lineItems = QuoteLineItemService.GetLineItems(quote.Id).ToList();
            var pagedLineItems = lineItems.ToPagedList(currentPage, perPage);
            var paginationUrl = Url.QuoteLineItems(quote.Id, quote.Name.UrlFriendly(), -1);

            var model = new QuoteLineItemsViewModel(quote, creator, pagedLineItems, currentPage, paginationUrl);
            return quote.Name.UrlFriendly() != name ? PageNotFound() : View(model);
        }

        [QuoteFlowRoute("quote/{id:INT}/{name}/change-history", Name = RouteNames.QuoteChangeHistory)]
        public virtual ActionResult ChangeHistory(int id, string name)
        {
            var quote = QuoteService.GetQuote(id);
            if (quote == null)
            {
                return PageNotFound();
            }

            // track that this quote has been visited
            UserTrackingService.UpdateRecentLinks(GetCurrentUser().Id, PageType.Quote, quote.Id, quote.Name);

            var creator = UserService.GetUser(quote.CreatorId);

            var model = new QuoteShowModel(quote, creator);
            return quote.Name.UrlFriendly() != name ? PageNotFound() : View(model);
        }

        [QuoteFlowRoute("quote/{id:INT}/{name}/access", Name = RouteNames.QuoteAccessControl)]
        public virtual ActionResult AccessControl(int id, string name)
        {
            var quote = QuoteService.GetQuote(id);
            if (quote == null)
            {
                return PageNotFound();
            }

            // track that this quote has been visited
            UserTrackingService.UpdateRecentLinks(GetCurrentUser().Id, PageType.Quote, quote.Id, quote.Name);

            var creator = UserService.GetUser(quote.CreatorId);

            var model = new QuoteShowModel(quote, creator);
            return quote.Name.UrlFriendly() != name ? PageNotFound() : View(model);
        }

        [QuoteFlowRoute("quote/{id:INT}/{name}/builder", Name = RouteNames.QuoteBuilder)]
        public virtual ActionResult ShowBuilder(int id, string name, string jql, int? selectedAssetId)
        {
            var quote = QuoteService.GetQuote(id);

            // track that this quote has been visited
            UserTrackingService.UpdateRecentLinks(GetCurrentUser().Id, PageType.Quote, quote.Id, quote.Name);

            if (selectedAssetId.HasValue)
            {
                AssetTableServiceConfiguration.SelectedAssetId = selectedAssetId.Value;
            }

            var assetTableOutcome = AssetTableService.GetIssueTableFromFilterWithJql(GetCurrentUser(), null, jql, AssetTableServiceConfiguration, true);
            if (!assetTableOutcome.IsValid())
            {
                throw new InvalidOperationException();
            }

            var assetTable = assetTableOutcome.ReturnedValue;

            var visibleFieldNames = new List<string>();
            var visibleFunctionNames = new List<string>();
            var jqlReservedWords = JqlStringSupport.GetJqlReservedWords();
            var lineItems = QuoteLineItemService.GetLineItems(quote.Id);

            var searchOutcome = SearcherService.SearchWithJql(GetCurrentUser(), jql ?? string.Empty, 0);

            var model = new QuoteBuilderViewModel(quote, assetTable.AssetTable, lineItems, visibleFieldNames, visibleFunctionNames,
                jqlReservedWords, searchOutcome, selectedAssetId);

            return quote.Name.UrlFriendly() != name ? PageNotFound() : View("ShowBuilder", model);
        }

        [QuoteFlowRoute("quote/{id:INT}/{name}/builder/{selectedAssetId:INT}", Name = RouteNames.QuoteBuilderWithSelectedAsset)]
        public virtual ActionResult ShowBuilderWithSelectedAsset(int id, string name, int selectedAssetId, string jql)
        {
            return ShowBuilder(id, name, jql, selectedAssetId);
        }
    }
}
