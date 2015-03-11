using Lucene.Net.Index;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Asset.Index
{
    public class AssetDocumentFactory : IAssetDocumentFactory
    {
        

        public Term GetIdentifyingTerm(IAsset asset)
        {
            throw new System.NotImplementedException();
        }
    }
}