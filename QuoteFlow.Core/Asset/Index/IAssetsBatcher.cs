using System.Collections.Generic;

namespace QuoteFlow.Core.Asset.Index
{
    /// <summary>
    /// Splits up a large set of assets into batches.
    /// </summary>
    public interface IAssetsBatcher : IEnumerable<IEnumerable<Api.Models.Asset>>
    {
    }
}