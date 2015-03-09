using Lucene.Net.Documents;
using QuoteFlow.Api.Asset;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Asset
{
    public class AssetFactory : IAssetFactory
    {
        public IAssetService AssetService { get; protected set; }
        public ICatalogService CatalogService { get; protected set; }
        public IFieldManager FieldManager { get; protected set; }

        public AssetFactory(IAssetService assetService, ICatalogService catalogService, IFieldManager fieldManager)
        {
            AssetService = assetService;
            CatalogService = catalogService;
            FieldManager = fieldManager;
        }

        public IAsset GetAsset(Document assetDocument)
        {
            return new DocumentAsset(assetDocument, FieldManager, AssetService, CatalogService);
        }
    }
}