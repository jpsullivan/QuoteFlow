using System;
using System.Linq;
using System.Web.Mvc;
using QuoteFlow.Api.Asset.Nav;
using QuoteFlow.Api.Configuration;
using QuoteFlow.Api.Infrastructure.Helpers;
using QuoteFlow.Api.Models.ViewModels.Quotes;
using QuoteFlow.Api.Services;
using QuoteFlow.Infrastructure;
using QuoteFlow.Infrastructure.Attributes;
using QuoteFlow.Infrastructure.Extensions;
using Route = QuoteFlow.Infrastructure.Attributes.RouteAttribute;

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
        public IManufacturerService ManufacturerService { get; protected set; }
        public IQuoteLineItemService QuoteLineItemService { get; protected set; }
        public IQuoteService QuoteService { get; protected set; }
        public IQuoteStatusService QuoteStatusService { get; protected set; }
        public IUserService UserService { get; protected set; }

        public QuoteController() { }

        public QuoteController(IAssetService assetService,
            IAssetTableService assetTableService,
            IAssetTableServiceConfiguration configuration,
            ICatalogService catalogService,
            IManufacturerService manufacturerService,
            IQuoteLineItemService quoteLineItemService,
            IQuoteService quoteService, 
            IQuoteStatusService quoteStatusService,
            IUserService userService)
        {
            AssetService = assetService;
            AssetTableService = assetTableService;
            AssetTableServiceConfiguration = configuration;
            CatalogService = catalogService;
            ManufacturerService = manufacturerService;
            QuoteService = quoteService;
            QuoteLineItemService = quoteLineItemService;
            QuoteStatusService = quoteStatusService;
            UserService = userService;
        }

        #endregion

        [Route("quotes", Name = RouteNames.QuoteIndex)]
        public virtual ActionResult Index()
        {
            var quotes = QuoteService.GetQuotesFromOrganization(CurrentOrganization.Id);
            var model = new QuotesViewModel(quotes);
            return View(model);
        }

        [Route("quote/new")]
        public virtual ActionResult New()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [Route("quote/create", HttpVerbs.Post)]
        public virtual ActionResult CreateQuote(NewQuoteModel model)
        {
            var currentUser = GetCurrentUser();

            // lazily set the organization to the first one that the user is assigned to.
            // eventually use whatever the users' current organization is once multi-tenancy
            // is implemented.
            model.Organization = currentUser.Organizations.First();

            if (!ModelState.IsValid) return New();

            // check to make sure that a quote with this name doesn't already exist
            if (QuoteService.ExistsWithinOrganization(model.QuoteName, CurrentOrganization.Id)) {
                var errorMsg = string.Format("Quote name already exists within the {0} organization.",
                    model.Organization.OrganizationName);
                ModelState.AddModelError("Name", errorMsg);
                return View("New", model);
            }

            var newQuote = QuoteService.CreateQuote(model, currentUser.Id);

            // there has to be a better way to do this...
            return Redirect("~/quote/" + newQuote.Id + "/" + newQuote.Name.UrlFriendly());
        }

        [Route("quote/{id:INT}/{name}", Name = RouteNames.QuoteShow)]
        public virtual ActionResult Show(int id, string name)
        {
            var quote = QuoteService.GetQuote(id);
            if (quote == null)
            {
                return PageNotFound();
            }

            var creator = UserService.GetUser(quote.CreatorId);
            var statuses = QuoteStatusService.GetStatuses(CurrentOrganization.Id);

            var model = new QuoteShowModel(quote, creator, statuses);
            return quote.Name.UrlFriendly() != name ? PageNotFound() : View(model);
        }

        [Route("quote/{id:INT}/{name}/line-items", Name = RouteNames.QuoteLineItems)]
        public virtual ActionResult LineItems(int id, string name, int? page)
        {
            var quote = QuoteService.GetQuote(id);
            if (quote == null)
            {
                return PageNotFound();
            }

            const int perPage = 50;
            var currentPage = Math.Max(page ?? 1, 1);

            var creator = UserService.GetUser(quote.CreatorId);
            var lineItems = QuoteLineItemService.GetLineItems(quote.Id).ToList();
            var pagedLineItems = lineItems.ToPagedList(currentPage, perPage);
            var paginationUrl = Url.QuoteLineItems(quote.Id, quote.Name.UrlFriendly(), -1);

            var model = new QuoteLineItemsViewModel(quote, creator, pagedLineItems, currentPage, paginationUrl);
            return quote.Name.UrlFriendly() != name ? PageNotFound() : View(model);
        }

        [Route("quote/{id:INT}/{name}/change-history", Name = RouteNames.QuoteChangeHistory)]
        public virtual ActionResult ChangeHistory(int id, string name)
        {
            var quote = QuoteService.GetQuote(id);
            if (quote == null)
            {
                return PageNotFound();
            }

            var creator = UserService.GetUser(quote.CreatorId);

            var model = new QuoteShowModel(quote, creator);
            return quote.Name.UrlFriendly() != name ? PageNotFound() : View(model);
        }

        [Route("quote/{id:INT}/{name}/access", Name = RouteNames.QuoteAccessControl)]
        public virtual ActionResult AccessControl(int id, string name)
        {
            var quote = QuoteService.GetQuote(id);
            if (quote == null)
            {
                return PageNotFound();
            }

            var creator = UserService.GetUser(quote.CreatorId);

            var model = new QuoteShowModel(quote, creator);
            return quote.Name.UrlFriendly() != name ? PageNotFound() : View(model);
        }

        [Route("quote/{id:INT}/{name}/builder", Name = RouteNames.QuoteBuilder)]
        public virtual ActionResult ShowBuilder(int id, string name)
        {
            var quote = QuoteService.GetQuote(id);
            var catalogs = CatalogService.GetCatalogs(CurrentOrganization.Id);
            var manufacturers = ManufacturerService.GetManufacturers(CurrentOrganization.Id);
            var creators = UserService.GetUsers(CurrentOrganization.Id);

            var assetTable = AssetTableService.GetIssueTableFromFilterWithJql(GetCurrentUser(), String.Empty, String.Empty,
                AssetTableServiceConfiguration, true);

            var model = new QuoteBuilderViewModel(quote, catalogs, manufacturers, creators, assetTable.AssetTable);

            return quote.Name.UrlFriendly() != name ? PageNotFound() : View(model);
        }
    }
}
