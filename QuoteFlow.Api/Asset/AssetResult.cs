using QuoteFlow.Api.Infrastructure.Services;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Asset
{
    /// <summary>
    /// A simple object that holds the information about an asset operation.
    /// </summary>
    public class AssetResult : ServiceResult
    {
        public Models.Asset Asset { get; protected set; }

        public AssetResult(Models.Asset asset, IErrorCollection errorCollection) : base(errorCollection)
        {
            Asset = asset;
        }
    }
}