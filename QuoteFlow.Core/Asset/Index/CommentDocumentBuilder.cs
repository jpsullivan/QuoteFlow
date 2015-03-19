using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Index.Indexers.Phrase;
using QuoteFlow.Api.Lucene.Building;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Util;

namespace QuoteFlow.Core.Asset.Index
{
    public class CommentDocumentBuilder : IEntityDocumentBuilder<AssetComment>
    {
        #region DI

        public IAssetService AssetService { get; set; }

        public CommentDocumentBuilder(IAssetService assetService)
        {
            AssetService = assetService;
        }

        #endregion

        public Document Build(AssetComment entity)
        {
            if (entity.Comment == null)
            {
                return null;
            }

            var asset = AssetService.GetAsset(entity.AssetId);

            var doc = new Document()
                .AddField(DocumentConstants.CatalogId, asset.CatalogId.ToString(), Field.Store.YES, Field.Index.ANALYZED_NO_NORMS)
                .AddField(DocumentConstants.AssetId, asset.Id.ToString(), Field.Store.YES, Field.Index.ANALYZED_NO_NORMS)
                .AddField(DocumentConstants.CommentId, entity.Id.ToString(), Field.Store.YES, Field.Index.ANALYZED_NO_NORMS)
                .AddField(DocumentConstants.CommentBody, entity.Comment, Field.Store.YES, Field.Index.ANALYZED)
                .AddField(PhraseQuerySupportField.ForIndexField(DocumentConstants.CommentBody), entity.Comment, Field.Store.YES, Field.Index.ANALYZED)
                .AddField(DocumentConstants.CommentCreated, LuceneUtils.DateToString(entity.CreatedUtc), Field.Store.YES, Field.Index.ANALYZED_NO_NORMS);

            var creator = entity.Creator;
            if (creator != null) // can't add null keywords
            {
                doc.AddField(DocumentConstants.CommentAuthor, creator.Id.ToString(), Field.Store.YES, Field.Index.ANALYZED_NO_NORMS);
            }

            return doc;
        }

        public IEnumerable<Document> Build(IEnumerable<AssetComment> entities)
        {
            return entities.Select(Build).ToList();
        }
    }
}