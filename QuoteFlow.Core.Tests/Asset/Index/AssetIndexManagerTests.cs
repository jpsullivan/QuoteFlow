using System.Collections.Generic;
using QuoteFlow.Api.Asset.Index;

namespace QuoteFlow.Core.Tests.Asset.Index
{
    public class AssetIndexManagerTests
    {
        private const string DEF_PROJECT_KEY = "ABC";
        private const string DEF_PROJECT_NAME = "A Project";
        private const string ISSUE1_ID = "1";
        private const string ISSUE2_ID = "2";
        private const string UNINDEXED = "UNINDEXED";

        private static readonly IDictionary<string, string> AssetDocument = new Dictionary<string, string>
        {
            { "id", DocumentConstants.AssetId },
            { "catalog", DocumentConstants.CatalogId },
            { "manufacturer", UNINDEXED }
        };


    }
}