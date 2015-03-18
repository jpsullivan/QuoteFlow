using System.Collections.Generic;
using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Asset.Index
{
    /// <summary>
    /// Get the documents (asset and comments) for the asset.
    /// </summary>
    public class DocumentCreationStrategy : IDocumentCreationStrategy
    {
        public IAssetDocumentFactory AssetDocumentFactory { get; protected set; }
        public IEntityDocumentBuilder<AssetComment> CommentDocumentBuilder { get; protected set; }

        public DocumentCreationStrategy(IAssetDocumentFactory assetDocumentFactory, IEntityDocumentBuilder<AssetComment> commentDocumentBuilder)
        {
            AssetDocumentFactory = assetDocumentFactory;
            CommentDocumentBuilder = commentDocumentBuilder;
        }

        public Documents Get(IAsset asset, bool includeComments)
        {
            var comments = includeComments ? CommentDocumentBuilder.Build(asset.Comments) : new List<Document>();
            return new Documents(asset, AssetDocumentFactory.);
        }
    }
}