using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace QuoteFlow.Api.Lucene.Index
{
    /// <summary>
    /// Partial interface of IndexWriter that contains only the methods we actually need.
    /// <p>
    /// Allows us to delegate much more easily and makes testing simpler. Also hides the
    /// implementation details of IndexWriter interaction.
    /// </summary>
    internal interface IWriter : IDisposable
    {
        void AddDocuments(ICollection<Document> document);

        void DeleteDocuments(Term identifyingTerm);

        void UpdateDocuments(Term identifyingTerm, ICollection<Document> document);

        /// <summary>
        /// Updates the document with the given {@code identifyingTerm} if and only if the value {@code optimisticLockField}
        /// in the index is equal to the value of {@code document.get(optimisticLockField)}. This is useful for achieving
        /// optimistic locking when updating the index (used in conjunction with a "version" or "updated date" field).
        /// </summary>
        /// <param name="identifyingTerm">A Term that uniquely identifies the document to be updated </param>
        /// <param name="document">A new Document </param>
        /// <param name="optimisticLockField">A String containing a field name </param>
        /// <exception cref="IOException"></exception>
        void UpdateDocumentConditionally(Term identifyingTerm, Document document, string optimisticLockField);

        void Optimize();

        void Close();

        void Commit();
    }
}
