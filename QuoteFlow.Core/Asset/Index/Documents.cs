using System.Collections.Generic;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Ninject;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.DependencyResolution;
using QuoteFlow.Core.Lucene;

namespace QuoteFlow.Core.Asset.Index
{
    public class Documents
    {
        [Inject]
        public IAssetDocumentFactory AssetDocumentFactory { get; set; }

        public Document Asset { get; private set; }
        public IEnumerable<Document> Comments { get; private set; }
        public Term IdentifyingTerm { get; private set; }

        public Documents(IAsset asset, Document issueDocument, IEnumerable<Document> comments)
        {
            var assetDocumentFactory = Container.Kernel.TryGet<AssetDocumentFactory>();

            Asset = issueDocument;
            Comments = LuceneDocumentsBuilder.BuildAll(comments);
            IdentifyingTerm = assetDocumentFactory.GetIdentifyingTerm(asset);
        }
    }
}