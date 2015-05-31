﻿using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Microsoft.Ajax.Utilities;
using QuoteFlow.Api.Infrastructure.Services;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Models.RequestModels;
using QuoteFlow.Api.Services;
using QuoteFlow.Infrastructure.Enumerables;
using Wintellect.PowerCollections;

namespace QuoteFlow.Controllers.Api
{
    public class AssetController : ApiController
    {
        #region DI

        public IAssetService AssetService { get; protected set; }
        public ISearcherService SearcherService { get; protected set; }
        public IUserService UserService { get; protected set; }

        public AssetController()
        {
        }

        public AssetController(IAssetService assetService, ISearcherService searcherService, IUserService userService)
        {
            AssetService = assetService;
            SearcherService = searcherService;
            UserService = userService;
        }

        #endregion

        public Asset Get(int id)
        {
            if (id == 0)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            return AssetService.GetAsset(id);
        }

        [HttpPost]
        public IEnumerable<Asset> FindAsset(AssetCriteriaQuery query)
        {
            return new List<Asset>();
            // todo: search controller logic, implement search service
        }

        [HttpPost]
        public IServiceOutcome<QuerySearchResults> QueryComponent()
        {
            // since post args can contain duplicate keys, we cannot rely on model binding
            var data = Request.Content.ReadAsStringAsync().Result;
            if (data.IsNullOrWhiteSpace())
            {
                return null;
            }

            var dataArray = data.Split('&');
            var dataList = new ListWithDuplicates();
            foreach (var arg in dataArray)
            {
                var args = arg.Split('=');
                dataList.Add(args[0], args[1]);
            }

            var multiDict = new MultiDictionary<string, string[]>(true);
            foreach (var arg in dataArray)
            {
                var args = arg.Split('=');
                multiDict.Add(args[0], new[] {args[1]});
            }

            // also get the user
            var user = UserService.GetUser(RequestContext.Principal.Identity.Name, null);

            return SearcherService.Search(user, multiDict, new long());
        }
    }
}