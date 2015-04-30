using System.Collections.Generic;
using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Asset.Index
{
    /// <summary>
    /// Get the documents (asset and comments) for the asset.
    /// </summary>
    public class DocumentCreationStrategy : IDocumentCreationStrategy
    {
        public IAssetDocumentFactory AssetDocumentFactory { get; protected set; }
        public IEntityDocumentBuilder<AssetComment> CommentDocumentBuilder { get; protected set; }
        public IAssetService AssetService { get; protected set; }

        public DocumentCreationStrategy(IAssetDocumentFactory assetDocumentFactory, IEntityDocumentBuilder<AssetComment> commentDocumentBuilder, IAssetService assetService)
        {
            AssetDocumentFactory = assetDocumentFactory;
            CommentDocumentBuilder = commentDocumentBuilder;
            AssetService = assetService;
        }

        public Documents Get(IAsset asset, bool includeComments)
        {
            var comments = new List<Document>();
            if (includeComments)
            {
                var assetComments = AssetService.GetAssetComments(asset.Id);
                CommentDocumentBuilder.Build(assetComments);
            }

            return new Documents(asset, AssetDocumentFactory.Apply(asset), comments);
        }
    }
}