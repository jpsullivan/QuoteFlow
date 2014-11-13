using Lucene.Net.Documents;
using QuoteFlow.Models.Assets.Search.Constants;

namespace QuoteFlow.Models.Assets.Index.Indexers
{
    /// <summary>
    /// Responsible for populating a <seealso cref="Document lucene document"/> with the information held in
    /// the &quot;Summary&quot; field of the <seealso cref="Asset asset"/>
    /// </summary>
    public class SummaryIndexer : BaseFieldIndexer
    {
        public SummaryIndexer() : base()
        {
        }

        public virtual string Id
        {
            get { return SystemSearchConstants.ForSummary().FieldId; }
        }

        public virtual string DocumentFieldId
        {
            get { return SystemSearchConstants.ForSummary().IndexField; }
        }

        public bool IsFieldVisibleAndInScope(Asset issue)
        {
            return true;
        }

        public virtual void AddIndex(Document doc, Asset asset)
        {
            IndexText(doc, DocumentFieldId, asset.Name, asset);
            IndexText(doc, PhraseQuerySupportField.forIndexField(DocumentFieldId), asset.Name, asset);
            IndexTextForSorting(doc, DocumentConstants.AssetSortName, asset.Name, asset);
        }
    }
}