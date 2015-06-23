﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Jil;
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

        public QuoteController(IAssetService assetService, IAssetTableService assetTableService, IAssetTableServiceConfiguration assetTableServiceConfiguration, ICatalogService catalogService, IJqlStringSupport jqlStringSupport, IManufacturerService manufacturerService, IQuoteLineItemService quoteLineItemService, IQuoteService quoteService, IQuoteStatusService quoteStatusService, ISearcherService searcherService, IUserService userService, IUserTrackingService userTrackingService)
        {
            AssetService = assetService;
            AssetTableService = assetTableService;
            AssetTableServiceConfiguration = assetTableServiceConfiguration;
            CatalogService = catalogService;
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
            return View();
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
        public virtual ActionResult ShowBuilder(int id, string name, string jql)
        {
            var quote = QuoteService.GetQuote(id);

            // track that this quote has been visited
            UserTrackingService.UpdateRecentLinks(GetCurrentUser().Id, PageType.Quote, quote.Id, quote.Name);

            var assetTable = AssetTableService.GetIssueTableFromFilterWithJql(GetCurrentUser(), null, jql, AssetTableServiceConfiguration, true);
            var visibleFieldNames = new List<string>();
            var visibleFunctionNames = new List<string>();
            var jqlReservedWords = JqlStringSupport.GetJqlReservedWords();

            var searchOutcome = SearcherService.SearchWithJql(GetCurrentUser(), jql, 0);

            var model = new QuoteBuilderViewModel(quote, assetTable.AssetTable, visibleFieldNames, visibleFunctionNames,
                jqlReservedWords, searchOutcome);

            return quote.Name.UrlFriendly() != name ? PageNotFound() : View(model);
        }
    }
}
