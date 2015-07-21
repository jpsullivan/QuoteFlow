using System;
using System.Collections.Generic;
using QuoteFlow.Api.Configuration.Lucene;
using QuoteFlow.Core.Configuration.Lucene;
using QuoteFlow.Core.Lucene.Index;

namespace QuoteFlow.Core.Asset.Index
{
    public class IndexDirectoryFactory : IIndexDirectoryFactory
    {
        public IIndexPathManager IndexPathManager { get; protected set; }
        public IIndexWriterConfiguration WriterConfiguration { get; protected set; }
        public IndexDirectoryFactoryMode Strategy = IndexDirectoryFactoryMode.Queued;

        public IndexDirectoryFactory(IIndexPathManager indexPathManager, IIndexWriterConfiguration writerConfiguration)
        {
            if (indexPathManager == null) throw new ArgumentNullException(nameof(indexPathManager));
            if (writerConfiguration == null) throw new ArgumentNullException(nameof(writerConfiguration));

            IndexPathManager = indexPathManager;
            WriterConfiguration = writerConfiguration;
        }

        public IDictionary<IndexDirectoryFactoryName, IIndexManager> Get()
        {
            var indexes = new Dictionary<IndexDirectoryFactoryName, IIndexManager>();
            foreach (IndexDirectoryFactoryName name in Enum.GetValues(typeof(IndexDirectoryFactoryName)))
            {
                var conf = new IndexConfiguration(
                    name.Directory(IndexPathManager), 
                    AssetIndexerAnalyzers.Indexing,
                    WriterConfiguration
                );
                indexes.Add(name, Strategy.CreateIndexManager(name.ToString(), conf));
            }

            return indexes;
        }

        public string IndexRootPath
        {
            get { return IndexPathManager.IndexRootPath; }
        }

        public IEnumerable<string> IndexPaths
        {
            get
            {
                var result = new List<string>();
                foreach (IndexDirectoryFactoryName indexType in Enum.GetValues(typeof(IndexDirectoryFactoryName)))
                {
                    try
                    {
                        result.Add(indexType.GetPath(IndexPathManager));
                    }
                    catch (Exception)
                    {
                        // probably not setup
                        throw;
                    }
                }

                return result;
            }
        }

        public IndexDirectoryFactoryMode IndexingMode
        {
            set { Strategy = value; }
        }
    }
}