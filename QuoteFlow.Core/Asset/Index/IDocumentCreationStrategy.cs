using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Asset.Index
{
    public interface IDocumentCreationStrategy
    {
        Documents Get(IAsset asset, bool includeComments);
    }
}