using Lucene.Net.Documents;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Api.Asset
{
    /// <summary>
    /// The AssetFactory is used for creating Assets in QuoteFlow, as well as converting <see cref="GenericValue"/> asset objects
    /// to proper <see cref="QuoteFlow.Api.Asset"/> objects. It only handles creational tasks. For update and retrieval see the <see cref="IAssetService"/> interface.
    /// </summary>
    public interface IAssetFactory
    {
        /// <summary>
        /// Creates an asset object for an asset represented by the Lucene Document.
        /// </summary>
        /// <param name="assetDocument"></param>
        /// <returns></returns>
        IAsset GetAsset(Document assetDocument);
    }
}