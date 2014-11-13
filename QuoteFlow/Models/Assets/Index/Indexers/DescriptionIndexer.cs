﻿using Lucene.Net.Documents;
using QuoteFlow.Models.Assets.Search.Constants;

namespace QuoteFlow.Models.Assets.Index.Indexers
{
    public class DescriptionIndexer : BaseFieldIndexer
    {
        public DescriptionIndexer() : base()
        {
        }

        public virtual string Id
        {
            get { return SystemSearchConstants.ForDescription().FieldId; }
        }

        public virtual string DocumentFieldId
        {
            get { return SystemSearchConstants.ForDescription().IndexField; }
        }

        public virtual void AddIndex(Document doc, Asset issue)
        {
            string descValue = issue.Description;
            IndexText(doc, DocumentFieldId, descValue, issue);
            IndexText(doc, PhraseQuerySupportField.forIndexField(DocumentFieldId), descValue, issue);
            IndexTextForSorting(doc, DocumentConstants.AssetSortDesc, descValue, issue);
        }
    }

}