using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Lucene.Building;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset.Index.Managers;

namespace QuoteFlow.Core.Asset.Index
{
    public class AssetDocumentBuilder : IEntityDocumentBuilder<Api.Models.Asset>
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

        public Document Build(Api.Models.Asset entity)
        {
            var doc = new Document()
                .AddAllIndexers(entity, FieldIndexerManager.AllAssetIndexers);

            return doc;
        }

        public IEnumerable<Document> Build(IEnumerable<Api.Models.Asset> entities)
        {
            return entities.Select(Build).ToList();
        }
    }
}