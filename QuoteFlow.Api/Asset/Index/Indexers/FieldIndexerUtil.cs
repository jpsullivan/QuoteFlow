using System;
using Lucene.Net.Documents;

namespace QuoteFlow.Api.Asset.Index.Indexers
{
    /// <summary>
    /// Used for doing simple indexing stuff.
    /// </summary>
    public class FieldIndexerUtil
    {
        private const int MAX_SORT_LENGTH = 50;

        public static void IndexKeywordWithDefault(Document doc, string indexField, long? aLong, string defaultValue)
        {
            string value = aLong != null ? aLong.ToString() : null;
            IndexKeywordWithDefault(doc, indexField, value, defaultValue);
        }

        public static void IndexKeywordWithDefault(Document doc, string indexField, string fieldValue, string defaultValue, bool searchable = true)
        {
            doc.Add(GetField(indexField, fieldValue, defaultValue, searchable));
        }

        private static Field GetField(string indexField, string fieldValue, string defaultValue, bool searchable)
        {
            string value = (fieldValue.HasValue()) ? fieldValue : defaultValue;
            Field.Index index = (searchable) ? Field.Index.NOT_ANALYZED_NO_NORMS : Field.Index.NO;
            return new Field(indexField, value, Field.Store.YES, index);
        }

        public static string GetValueForSorting(string fieldValue)
        {
            string trimmed = (fieldValue == null) ? null : fieldValue.Trim();
            
            if (!trimmed.HasValue()) return Convert.ToString('\ufffd');

            return trimmed.Length > MAX_SORT_LENGTH ? trimmed.Substring(0, MAX_SORT_LENGTH) : trimmed;
        }
    }
}