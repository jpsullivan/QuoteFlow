using System;
using System.Globalization;
using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Index.Indexers;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Core.Util;

namespace QuoteFlow.Core.Asset.Index.Indexers
{
    public abstract class BaseFieldIndexer : IFieldIndexer
    {
        internal Field.Index Unanalyzed(Api.Models.Asset asset)
        {
            return (IsFieldVisibleAndInScope(asset)) ? Field.Index.NOT_ANALYZED_NO_NORMS : Field.Index.NO;
        }

        /// <summary>
        /// Index a single keyword field </summary>
        /// <param name="doc"> the document to add the field to. </param>
        /// <param name="indexField"> the document field name to user. </param>
        /// <param name="fieldValue"> the value to index. This value will NOT be folded before adding it to the document. </param>
        /// <param name="asset">The asset that defines the context and contains the value we are indexing. </param>
        public void IndexKeyword(Document doc, string indexField, string fieldValue, Api.Models.Asset asset)
        {
            if (fieldValue.HasValue())
            {
                doc.Add(new Field(indexField, fieldValue, Field.Store.YES, Unanalyzed(asset)));
            }
        }

        /// <summary>
        /// Case fold the passed keyword and add it to the passed document.
        /// </summary>
        /// <param name="doc">The document to add the field to.</param>
        /// <param name="indexField">The document field name to use.</param>
        /// <param name="fieldValue">The value to index. This value will be folded before adding it to the document.</param>
        /// <param name="asset">The asset that defines the context and contains the value we are indexing.</param>
        public void IndexFoldedKeyword(Document doc, string indexField, string fieldValue, Api.Models.Asset asset)
        {
            if (fieldValue.HasValue())
            {
                if (IsFieldVisibleAndInScope(asset))
                {
                    doc.Add(new Field(indexField, fieldValue.ToLower(CultureInfo.CurrentCulture), Field.Store.NO, Field.Index.ANALYZED_NO_NORMS));
                }
                // since we don't store we don't need to add it at all if it is not visible
            }
        }

        /// <summary>
        /// Index a single keyword field, with a default if the asset field is not set
        /// 
        /// shared with CommentDocumentFactory
        /// </summary>
        public void IndexKeywordWithDefault(Document doc, string indexField, string fieldValue, string defaultValue, Api.Models.Asset asset)
        {
            FieldIndexerUtil.IndexKeywordWithDefault(doc, indexField, fieldValue, defaultValue, IsFieldVisibleAndInScope(asset));
        }

        public void IndexKeywordWithDefault(Document doc, string indexField, long? aLong, string defaultValue, Api.Models.Asset asset)
        {
            string value = aLong?.ToString();
            IndexKeywordWithDefault(doc, indexField, value, defaultValue, asset);
        }

        /// <summary>
        /// Index a single text field
        /// </summary>
        public void IndexText(Document doc, string indexField, string fieldValue, Api.Models.Asset asset)
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
        public void IndexDateField(Document doc, string indexField, DateTime date, Api.Models.Asset asset)
        {
            Field.Index indexType = Unanalyzed(asset);
            doc.Add(new Field(indexField, LuceneUtils.DateToString(date), Field.Store.YES, indexType));
            if (indexType == Field.Index.NOT_ANALYZED_NO_NORMS)
            {
                doc.Add(new Field(DocumentConstants.LuceneSortFieldPrefix + indexField, LuceneUtils.DateToString(date), Field.Store.NO, Unanalyzed(asset)));
            }
        }

        /// <summary>
        /// Index a single text field
        /// </summary>
        public void IndexTextForSorting(Document doc, string indexField, string fieldValue, Api.Models.Asset asset)
        {
            string sortingValue = FieldIndexerUtil.GetValueForSorting(fieldValue);
            if (sortingValue.HasValue() && IsFieldVisibleAndInScope(asset))
            {
                doc.Add(new Field(indexField, sortingValue, Field.Store.NO, Field.Index.NOT_ANALYZED_NO_NORMS));
            }
        }

        public virtual string Id { get; private set; }
        public virtual string DocumentFieldId { get; private set; }

        public abstract void AddIndex(Document doc, Api.Models.Asset asset);

        public virtual bool IsFieldVisibleAndInScope(Api.Models.Asset asset)
        {
            return true;
        }

        protected bool Equals(BaseFieldIndexer other)
        {
            return string.Equals(Id, other.Id) && string.Equals(DocumentFieldId, other.DocumentFieldId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BaseFieldIndexer) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Id?.GetHashCode() ?? 0)*397) ^ (DocumentFieldId?.GetHashCode() ?? 0);
            }
        }
    }
}