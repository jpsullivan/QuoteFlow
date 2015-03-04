using System;
using System.Collections.Generic;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Nav;
using QuoteFlow.Api.Configuration;
using QuoteFlow.Api.Models.ViewModels.Assets.Nav;

namespace QuoteFlow.Core.Asset.Nav
{
    public class AssetTableService : IAssetTableService
    {
        private const string ColumnNames = "columnNames";
        private const int MaxJqlErrors = 10;

        public IFieldManager FieldManager { get; protected set; }
        

        public AssetTableViewModel GetIssueTableFromFilterWithJql(string filterId, string jql, IAssetTableServiceConfiguration config,
            bool isStableSearchFirstHit)
        {
            throw new NotImplementedException();
        }

        public AssetTableViewModel GetAssetTableFromAssetIds(string filterId, string jql, List<int> ids,
            IAssetTableServiceConfiguration config)
        {
            throw new NotImplementedException();
        }
    }
}
