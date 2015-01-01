using System;
using Lucene.Net.Search;
using QuoteFlow.Api.Lucene.Index;

namespace QuoteFlow.Core.Lucene.Index
{
    /// <summary>
    /// An <see cref="IEngine"/> maintains the lifecycle of a <see cref="Writer"/> while allowing 
    /// access to it via a <seealso cref="Function"/> that is used to add and delete documents to it.
    /// </summary>
    public interface IEngine : IDisposable
    {
        void Write(IndexOperation operation);

        /// <summary>
        /// Wait until any open Searchers are closed and then close the connection.
        /// </summary>
        void Close();

        /// <summary>
        /// Get the current searcher. Must be closed after use.
        /// </summary>
        IndexSearcher Searcher { get; }

        /// <summary>
        /// Clean (wipe) the index.
        /// </summary>
        void Clean();
    }
}