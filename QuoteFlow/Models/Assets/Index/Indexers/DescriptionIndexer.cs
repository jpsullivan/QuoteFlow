using Lucene.Net.Documents;
using QuoteFlow.Models.Assets.Index.Indexers.Phrase;
using QuoteFlow.Models.Assets.Search.Constants;

namespace QuoteFlow.Models.Assets.Index.Indexers
{
    public class DescriptionIndexer : BaseFieldIndexer
    {
        public virtual string Id
        {
            get { return SystemSearchConstants.ForDescription().FieldId; }
        }

        public virtual string DocumentFieldId
        {
            get { return SystemSearchConstants.ForDescription().IndexField; }
        }

        public override void AddIndex(Document doc, Asset issue)
        {
            string descValue = issue.Description;
            IndexText(doc, DocumentFieldId, descValue, issue);
            IndexText(doc, PhraseQuerySupportField.ForIndexField(DocumentFieldId), descValue, issue);
            IndexTextForSorting(doc, DocumentConstants.AssetSortDesc, descValue, issue);
        }
    }
}