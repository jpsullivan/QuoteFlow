using System;
using System.Collections.Generic;
using Lucene.Net.Store;
using QuoteFlow.Api.Configuration;
using QuoteFlow.Api.Configuration.Lucene;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.Configuration.Lucene;
using QuoteFlow.Core.Index;
using QuoteFlow.Core.Lucene.Index;

namespace QuoteFlow.Core.Asset.Index
{
    public interface IIndexDirectoryFactory : ISupplier<IDictionary<IndexDirectoryFactoryName, IIndexManager>>
    {
        string IndexRootPath { get; }

        IEnumerable<string> IndexPaths { get; }

        /// <summary>
        /// Gets/Sets the Indexing Mode - one of either DIRECT or QUEUED.
        /// </summary>
        IndexDirectoryFactoryMode IndexingMode { set; }
    }

    public enum IndexDirectoryFactoryMode
	{
		Direct,
		Queued
	}

    public static class IndexDirectoryFactoryModeExtensions
    {
        public static IIndexManager CreateIndexManager(this IndexDirectoryFactoryMode mode, string name, IIndexConfiguration configuration)
        {
            if (mode == IndexDirectoryFactoryMode.Direct)
            {
                return Indexes.CreateSimpleIndexManager(configuration);
            }

            if (mode == IndexDirectoryFactoryMode.Queued)
            {
                const int maxQueueSize = 1000;
                return Indexes.CreateQueuedIndexManager(mode.ToString(), configuration, maxQueueSize);
            }

            throw new InvalidOperationException("Could not imeplement behavior based on mode.");
        }
    }

    public enum IndexDirectoryFactoryName
    {
        Asset,
        Comment
    }

    public static class IndexDirectoryFactoryNameExtensions
    {
        public static Directory Directory(this IndexDirectoryFactoryName name, IIndexPathManager indexPathManager)
        {
            // todo: skipping the robust version of this and forcibly defaulting to AppData for now.
            return IndexPathManager.GetDirectory(LuceneIndexLocation.AppData);
        }

        public static string Verify(this IndexDirectoryFactoryName name, IIndexPathManager pathManager, string path)
        {
            if (pathManager.Mode == IndexPathManagerMode.Disabled)
            {
                throw new InvalidOperationException("Indexing is disabled.");
            }

            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            return path;
        }

        public static string GetPath(this IndexDirectoryFactoryName name, IIndexPathManager indexPathManager)
        {
            if (name == IndexDirectoryFactoryName.Asset)
            {
                return Verify(name, indexPathManager, indexPathManager.AssetIndexPath);
            }

            if (name == IndexDirectoryFactoryName.Comment)
            {
                return Verify(name, indexPathManager, indexPathManager.CommentIndexPath);
            }

            throw new InvalidOperationException("No job found for this index directory factory name: " + name);
        }
    }
}