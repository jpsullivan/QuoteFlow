﻿using System;
using Lucene.Net.Search;

namespace QuoteFlow.Core.Lucene.Index
{
    /// <summary>
    /// Management of an <see cref="Index"/>
    /// </summary>
    public interface IIndexManager : IDisposable
    {
        /// <summary>
        /// Get the current IndexConnection this manager holds. May throw
        /// exceptions if the index has not been created.
        /// </summary>
        /// <returns>The Index this manager refers to.</returns>
        IIndex Index { get; }

        /// <summary>
        /// Get the current <see cref="IndexSearcher"/> from the <see cref="Index"/>.
        /// 
        /// You must call the <see cref="IndexSearcher#close() close"/> method in a
        /// finally block once the searcher is no longer needed.
        /// </summary>
        /// <returns> the current <see cref="IndexSearcher"/></returns>
        IndexSearcher OpenSearcher();

        /// <summary>
        /// Returns true if the index has been created. This means that the index
        /// directory itself exists AND has been initialised with the default
        /// required index files.
        /// </summary>
        /// <returns> true if the index exists, false otherwise. </returns>
        bool IndexCreated { get; }

        /// <summary>
        /// Clean out the underlying directory the index is contained in.
        /// Blow away any indexes that currently live there.
        /// </summary>
        void DeleteIndexDirectory();
    }
}
