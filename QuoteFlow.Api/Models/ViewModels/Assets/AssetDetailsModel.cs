using System.Collections.Generic;
using QuoteFlow.Api.Auditing;

namespace QuoteFlow.Api.Models.ViewModels.Assets
{
    public class AssetDetailsModel
    {
        public AssetDetailsModel()
        {
        }

        public AssetDetailsModel(Asset asset, IEnumerable<AuditLogRecord> assetHistory, bool builderEnabled)
        {
            Asset = asset;
            AssetHistory = assetHistory;
            BuilderEnabled = builderEnabled;
        }

        public Asset Asset { get; set; }
        public IEnumerable<AuditLogRecord> AssetHistory { get; set; } 
        public bool BuilderEnabled { get; set; }
    }
}
