using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Index.Indexers.Phrase;
using QuoteFlow.Api.Asset.Search.Constants;

namespace QuoteFlow.Core.Asset.Index.Indexers
{
    public class DescriptionIndexer : BaseFieldIndexer
    {
        public override string Id => SystemSearchConstants.ForDescription().FieldId;

        public override string DocumentFieldId => SystemSearchConstants.ForDescription().IndexField;

        public override void AddIndex(Document doc, Api.Models.Asset issue)
        {
            string descValue = issue.Description;
            IndexText(doc, DocumentFieldId, descValue, issue);
            IndexText(doc, PhraseQuerySupportField.ForIndexField(DocumentFieldId), descValue, issue);
            IndexTextForSorting(doc, DocumentConstants.AssetSortDesc, descValue, issue);
        }
    }
}