using Lucene.Net.Documents;
using Lucene.Net.Index;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Asset.Index.Managers;
using QuoteFlow.Core.Index;

namespace QuoteFlow.Core.Asset.Index
{
    public class AssetDocumentFactory : IAssetDocumentFactory
    {
        public IEntityDocumentBuilder<IAsset> AssetDocumentBuilder { get; protected set; }
        public IFieldIndexerManager FieldIndexerManager { get; protected set; }
        public ISearchExtractorRegistrationManager SearchExtractorManager { get; protected set; }

        public AssetDocumentFactory(IEntityDocumentBuilder<IAsset> assetDocumentBuilder, IFieldIndexerManager fieldIndexerManager, ISearchExtractorRegistrationManager searchExtractorManager)
        {
            AssetDocumentBuilder = assetDocumentBuilder;
            FieldIndexerManager = fieldIndexerManager;
            SearchExtractorManager = searchExtractorManager;
        }

        public Document Apply(IAsset asset)
        {
            return AssetDocumentBuilder.Build(asset);
        }

        public Term GetIdentifyingTerm(IAsset asset)
        {
            return new Term(DocumentConstants.AssetId, asset.Id.ToString());
        }
    }
}