using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using QuoteFlow.Api.Asset.Fields.Layout.Column;
using QuoteFlow.Api.Asset.Nav;
using QuoteFlow.Api.Configuration;
using QuoteFlow.Api.Infrastructure.Services;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Models.ViewModels.Assets.Nav;
using QuoteFlow.Core.Configuration;

namespace QuoteFlow.Controllers.Api
{
    public class AssetTableController : ApiController
    {
        #region DI

        public IAssetTableService AssetTableService { get; protected set; }
        public IStableSearchService StableSearchService { get; protected set; }

        public AssetTableController()
        {
        }

        public AssetTableController(IAssetTableService assetTableService, IStableSearchService stableSearchService)
        {
            AssetTableService = assetTableService;
            StableSearchService = stableSearchService;
        }

        #endregion

        // GET: api/AssetTable
        public AssetTableViewModel Post([FromBody]AssetTableRequest model)
        {
            if (model.FilterId == null && model.Jql == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var config = BuildConfiguration(model, null);
            var outcome = AssetTableService.GetIssueTableFromFilterWithJql(new User(), string.Empty, model.Jql, config, true);

            if (outcome.IsValid())
            {
                return outcome.ReturnedValue;
            }

            var errors = outcome.ErrorCollection;
            throw new HttpResponseException(HttpStatusCode.BadRequest);
        }

        [HttpPost]
        public StableSearchResult Stable([FromBody] AssetTableRequest request)
        {
            var config = BuildConfiguration(request, null);
            var outcome = StableSearchService.GetAssetTableFromAssetIds(request.FilterId.ToString(), request.Jql, request.Id, config);
            if (outcome.IsValid())
            {
                return outcome.ReturnedValue;
                //return Response.ok(outcome.getReturnedValue()).cacheControl(never()).build();
            }

            var errors = outcome.ErrorCollection;
            throw new HttpResponseException(HttpStatusCode.BadRequest);
        } 

        private static IAssetTableServiceConfiguration CreateConfig(AssetTableRequest model, ColumnConfig columnConfig, List<string> columnNames)
        {
            var config = new AssetTableServiceConfiguration
            {
                EnableSorting = true,
                LayoutKey = model.LayoutKey,
                Start = model.StartIndex,
                ColumnNames = columnNames,
                ColumnConfig = columnConfig
            };

            // if it was explicitly requested that we display a certain number
            // of assets, then do that; otherwise fall back to the user's preferences.
            if (model.Num != null)
            {
                config.NumberToShow = Convert.ToInt32(model.Num);
            }
            else
            {
                // todo: make this configurable per user
                config.NumberToShow = 50;
            }

            return config;
        }

        private static IAssetTableServiceConfiguration BuildConfiguration(AssetTableRequest model, List<string> explicitColumns)
        {
            var columnConfig = ColumnConfig.None;
            var config = CreateConfig(model, columnConfig, null);

            if (columnConfig == ColumnConfig.Explicit)
            {
                config.ColumnNames = explicitColumns;
            }

            return config;
        }
    }

    public class AssetTableRequest
    {
        public int? FilterId { get; set; }
        public string Jql { get; set; }
        public string Num { get; set; }
        public int StartIndex { get; set; }
        public string LayoutKey { get; set; }
        public string ColumnConfig { get; set; }
        public List<int?> Id { get; set; } 
    }
}
