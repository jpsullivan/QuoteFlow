using Lucene.Net.Documents;
using Lucene.Net.Index;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Asset.Index
{
    /// <summary>
    /// Abstracts the means to create a <see cref="Document"/> for an <see cref="IAsset"/>.
    /// </summary>
    public interface IAssetDocumentFactory
    {
        Term GetIdentifyingTerm(IAsset asset);
    }
}