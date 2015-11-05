using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Lucene.Building;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset.Index.Managers;

namespace QuoteFlow.Core.Asset.Index
{
    public class AssetDocumentBuilder : IEntityDocumentBuilder<IAsset>
    {
        #region DI

        public IAssetService AssetService { get; set; }
        public IFieldIndexerManager FieldIndexerManager { get; set; }

        public AssetDocumentBuilder(IAssetService assetService, IFieldIndexerManager fieldIndexerManager)
        {
            AssetService = assetService;
            FieldIndexerManager = fieldIndexerManager;
        }

        #endregion

        public Document Build(IAsset entity)
        {
            return new Document().AddAllIndexers(entity, FieldIndexerManager.AllAssetIndexers);
        }

        public IEnumerable<Document> Build(IEnumerable<IAsset> entities)
        {
            return entities.Select(Build).ToList();
        }
    }
}