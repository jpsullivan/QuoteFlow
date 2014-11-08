using System;
using Lucene.Net.Documents;
using QuoteFlow.Infrastructure.Extensions;

namespace QuoteFlow.Models.Assets.Index.Indexers
{
    public abstract class BaseFieldIndexer : IFieldIndexer
    {
        internal virtual Field.Index Unanalyzed(Asset asset)
        {
            return (IsFieldVisibleAndInScope(asset)) ? Field.Index.NOT_ANALYZED_NO_NORMS : Field.Index.NO;
        }

        /// <summary>
        /// Index a single keyword field </summary>
        /// <param name="doc"> the document to add the field to. </param>
        /// <param name="indexField"> the document field name to user. </param>
        /// <param name="fieldValue"> the value to index. This value will NOT be folded before adding it to the document. </param>
        /// <param name="asset">The asset that defines the context and contains the value we are indexing. </param>
        public virtual void IndexKeyword(Document doc, string indexField, string fieldValue, Asset asset)
        {
            if (fieldValue.HasValue())
            {
                doc.Add(new Field(indexField, fieldValue, Field.Store.YES, Unanalyzed(asset)));
            }
        }

        /// <summary>
        /// Index a single keyword field, with a default if the asset field is not set
        /// 
        /// shared with CommentDocumentFactory
        /// </summary>
        public virtual void IndexKeywordWithDefault(Document doc, string indexField, string fieldValue, string defaultValue, Asset asset)
        {
            FieldIndexerUtil.IndexKeywordWithDefault(doc, indexField, fieldValue, defaultValue, IsFieldVisibleAndInScope(asset));
        }

        public virtual void IndexKeywordWithDefault(Document doc, string indexField, long? aLong, string defaultValue, Asset asset)
        {
            string value = aLong != null ? aLong.ToString() : null;
            IndexKeywordWithDefault(doc, indexField, value, defaultValue, asset);
        }

        /// <summary>
        /// Index a single text field
        /// </summary>
        public virtual void IndexText(Document doc, string indexField, string fieldValue, Asset asset)
        {
            if (fieldValue.HasValue())
            {
                doc.Add(IsFieldVisibleAndInScope(asset)
                    ? new Field(indexField, fieldValue, Field.Store.YES, Field.Index.ANALYZED)
                    : new Field(indexField, fieldValue, Field.Store.YES, Field.Index.NO));
            }
        }

        /// <summary>
        /// Index a single keyword field, with a date-time value
        /// </summary>
        public virtual void IndexDateField(Document doc, string indexField, DateTime date, Asset asset)
        {
            Field.Index indexType = Unanalyzed(asset);
            if (date != null)
            {
                doc.Add(new Field(indexField, LuceneUtils.dateToString(date), Field.Store.YES, indexType));
            }
            if (indexType == Field.Index.NOT_ANALYZED_NO_NORMS)
            {
                doc.Add(new Field(DocumentConstants.LuceneSortFieldPrefix + indexField, LuceneUtils.dateToString(date), Field.Store.NO, Unanalyzed(asset)));
            }
        }

        /// <summary>
        /// Index a single text field
        /// </summary>
        public virtual void IndexTextForSorting(Document doc, string indexField, string fieldValue, Asset asset)
        {
            string sortingValue = FieldIndexerUtil.GetValueForSorting(fieldValue);
            if (sortingValue.HasValue() && IsFieldVisibleAndInScope(asset))
            {
                doc.Add(new Field(indexField, sortingValue, Field.Store.NO, Field.Index.NOT_ANALYZED_NO_NORMS));
            }
        }

        public string Id { get; private set; }
        public string DocumentFieldId { get; private set; }

        public void AddIndex(Document doc, Asset asset)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsFieldVisibleAndInScope(Asset asset)
        {
            return true;
        }
    }
}