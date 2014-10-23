using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Microsoft.Ajax.Utilities;
using QuoteFlow.Infrastructure.Enumerables;
using QuoteFlow.Models;
using QuoteFlow.Models.RequestModels;
using QuoteFlow.Models.Search.Jql;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Controllers.Api
{
    public class AssetController : ApiController
    {
        #region IoC

        protected IAssetService AssetService { get; set; }
        protected IAssetSearchService AssetSearchService { get; set; }

        public AssetController() { }

        public AssetController(IAssetService assetService,
            IAssetSearchService assetSearchService)
        {
            AssetService = assetService;
            AssetSearchService = assetSearchService;
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
        public SearchResults QueryComponent()
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

            return AssetSearchService.Search(new Dictionary<string, string[]>(), new long());
        }
    }
}