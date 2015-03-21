using System.Collections.Generic;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Asset.Index
{
    /// <summary>
    /// Splits up a large set of assets into batches.
    /// </summary>
    public interface IAssetsBatcher : IEnumerable<IAsset>
    {
    }
}