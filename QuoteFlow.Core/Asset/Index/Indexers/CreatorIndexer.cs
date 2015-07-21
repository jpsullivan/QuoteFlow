using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Search.Constants;

namespace QuoteFlow.Core.Asset.Index.Indexers
{
    /// <summary>
    /// Class used for indexing the CreatorSystemField.
    /// </summary>
    public class CreatorIndexer : UserFieldIndexer
    {
        public override string Id => SystemSearchConstants.ForCreator().FieldId;

        public override string DocumentFieldId => SystemSearchConstants.ForCreator().IndexField;

        public override void AddIndex(Document doc, Api.Models.Asset asset)
        {
            IndexUserkeyWithDefault(doc, DocumentFieldId, asset.CreatorId.ToString(), SystemSearchConstants.ForCreator().EmptyIndexValue, asset);
        }
    }
}