﻿using System;
using Lucene.Net.Search;
using QuoteFlow.Core.Index;

namespace QuoteFlow.Core.Lucene.Index
{
    /// <summary>
    /// An <see cref="IEngine"/> maintains the lifecycle of a <see cref="IWriter"/> while allowing 
    /// access to it via a <see cref="Function"/> that is used to add and delete documents to it.
    /// </summary>
    public interface IEngine : IDisposable
    {
        void Write(Operation operation);

        /// <summary>
        /// Get the current searcher. Must be closed after use.
        /// </summary>
        IndexSearcher GetSearcher();

        /// <summary>
        /// Clean (wipe) the index.
        /// </summary>
        void Clean();
    }
}