using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Index.Indexers;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Lucene.Building
{
    public static class DocumentExtensions
    {
        /// <summary>
		/// Adds a field to the index analysing its content according to the index writer's analyzer
		/// Standard Analyzer will remove much of the punctation
		/// </summary>
		/// <param name="document">The document to add the field to </param>
		/// <param name="fieldName">The name of the field to add</param>
		/// <param name="value">The value of the field</param>
		/// <param name="store">A boolean denoting whether to store the value in the index - allows retrieval of the original value from the index</param>
		/// <param name="caseSensitive">Whether to store the value in its original case</param>
		/// <returns>The input document object</returns>
		public static Document AddAnalysedField(this Document document, string fieldName, string value, bool store = false, bool caseSensitive = false)
		{
			Field.Store luceneStore = GetStoreValue(store);
			return AddField(document, fieldName, value, caseSensitive, luceneStore, Field.Index.ANALYZED);
		}

		/// <summary>
		/// Adds a non analysed field to the index - the field acts as a complete value in the index, therefore will not be stripped of punctuation / whitespace
		/// and can be searched in its original entirity
		/// </summary>
		/// <param name="document">The document to add the field to </param>
		/// <param name="fieldName">The name of the field to add</param>
		/// <param name="value">The value of the field</param>
		/// <param name="store">A boolean denoting whether to store the value in the index - allows retrieval of the original value from the index</param>
		/// <param name="caseSensitive">Whether to store the value in its original case</param>
		/// <returns>The input document object</returns>
		public static Document AddNonAnalysedField(this Document document, string fieldName, string value, bool store = false, bool caseSensitive = false)
		{
			Field.Store luceneStore = GetStoreValue(store);
			return AddField(document, fieldName, value, caseSensitive, luceneStore, Field.Index.NOT_ANALYZED);
		}

		/// <summary>
		/// Stores the value in the index without indexing it - RETRIEVAL FIELDS ONLY
		/// </summary>
		/// <param name="document">The document to add the field to </param>
		/// <param name="fieldName">The name of the field to add</param>
		/// <param name="value">The value of the field</param>
		/// <returns>The input document object</returns>
		public static Document AddStoredField(this Document document, string fieldName, string value)
		{
			return AddField(document, fieldName, value, true, Field.Store.YES, Field.Index.NO);
		}

        /// <summary>
        /// Adds a field to the index, bit more configurable than the other helper methods, but more verbose as a consequence.
        /// </summary>
        /// <param name="document">The document to add the field to </param>
        /// <param name="fieldName">The name of the field to add</param>
        /// <param name="value">The value of the field</param>
        /// <param name="store">A boolean denoting whether to store the value in the index - allows retrieval of the original value from the index</param>
        /// <param name="index">The type of indexing to apply to the field</param>
        /// <returns>The input document object</returns>
        public static Document AddField(this Document document, string fieldName, string value, Field.Store store, Field.Index index)
        {
            return AddField(document, fieldName, value, false, store, index);
        }

		/// <summary>
		/// Adds a field to the index, bit more configurable than the other helper methods, but more verbose as a consequence.
		/// </summary>
		/// <param name="document">The document to add the field to </param>
		/// <param name="fieldName">The name of the field to add</param>
		/// <param name="value">The value of the field</param>
		/// <param name="store">A boolean denoting whether to store the value in the index - allows retrieval of the original value from the index</param>
		/// <param name="caseSensitive">Whether to store the value in its original case</param>
		/// <param name="index">The type of indexing to apply to the field</param>
		/// <returns>The input document object</returns>
		public static Document AddField(this Document document, string fieldName, string value, bool caseSensitive, Field.Store store, Field.Index index)
		{
			if (value == null || string.IsNullOrEmpty(fieldName))
			{
				return document;
			}

			if (!caseSensitive)
			{
				value = value.ToLower();
			}

			Field field = new Field(fieldName, value, store, index);
			document.Add(field);
			return document;
		}

        public static Document AddAllIndexers(this Document document, IAsset asset, IEnumerable<IFieldIndexer> indexers)
        {
            var visibleDocumentFieldIds = indexers.Select(indexer => AddIndexer(document, asset, indexer)).ToList();

            GenerateNonEmptyFieldIds(document);

            foreach (var fieldId in visibleDocumentFieldIds)
            {
                document.AddField("visiblefieldids", fieldId, Field.Store.NO, Field.Index.NOT_ANALYZED_NO_NORMS);
            }

            return document;
        }

        private static string AddIndexer(Document document, IAsset asset, IFieldIndexer indexer)
        {
            var resolvedAsset = (Models.Asset) asset;
            try
            {
                var documentFieldId = indexer.DocumentFieldId;
                indexer.AddIndex(document, resolvedAsset);
                if (indexer.IsFieldVisibleAndInScope(resolvedAsset))
                {
                    return documentFieldId;
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //DropField(documentFieldId, indexer, re);
            }

            return null;
        }

        /// <summary>
        /// Get all of the fields in the document and add a new field whose value is the
        /// name of all the included fields.
        /// </summary>
        private static void GenerateNonEmptyFieldIds(Document doc)
        {
            foreach (var fieldName in GetNonEmptyFieldNames(doc))
            {
                doc.AddField(DocumentConstants.AssetNonEmptyFieldIds, fieldName, Field.Store.NO,
                    Field.Index.NOT_ANALYZED_NO_NORMS);
            }
        }

        private static IEnumerable<string> GetNonEmptyFieldNames(Document doc)
        {
            var fields = doc.GetFields();
            var names = new List<string>(fields.Count);

            // NOTE: we do not store the field value since we are never interested in 
            // reading the value out of the document, we are just interested in searching it.
            // This will keep us from adding to the size of the asset document.
            names.AddRange(from field in fields where field.IsIndexed select field.Name);
            return names;
        } 

		/// <summary>
		/// Sets up an already existing document with the specified actions
		/// </summary>
		/// <param name="document"></param>
		/// <param name="documentActions"></param>
		/// <returns></returns>
		public static Document Setup(this Document document, params Action<Document>[] documentActions)
		{
			foreach (Action<Document> item in documentActions)
			{
				item(document);
			}
			return document;
		}

		#region Helpers


		private static Field.Store GetStoreValue(bool store)
		{
			Field.Store luceneStore = Field.Store.NO;
			if (store)
			{
				luceneStore = Field.Store.YES;
			}
			return luceneStore;
		}

		#endregion
    }
}