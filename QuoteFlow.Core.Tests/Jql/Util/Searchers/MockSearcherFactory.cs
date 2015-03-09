using System;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using QuoteFlow.Api.Lucene.Index;
using Directory = Lucene.Net.Store.Directory;

namespace QuoteFlow.Core.Tests.Jql.Util.Searchers
{
    /// <summary>
    /// Handy methods for creating searcher instances in tests.
    /// </summary>
    public class MockSearcherFactory
    {
        public static Directory GetCleanRamDirectory()
        {
            try
            {
                var directory = new RAMDirectory();
                // todo: lucene 4.8 only
//                var config = new DefaultIndexWriterConfiguration(LuceneVersion.Get(), new StandardAnalyzer(LuceneVersion.Get()));
//                config.OpenMode = IndexWriterConfig.OpenMode.Create;
                Analyzer analyzer = new StandardAnalyzer(LuceneVersion.Get());
                (new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED)).Dispose();
                return directory;
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }

        public static IndexSearcher GetSearcher(Directory directory)
        {
            try
            {
                return new IndexSearcher(directory);
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }

        public static IndexSearcher GetCleanSearcher()
        {
            return GetSearcher(GetCleanRamDirectory());
        }
    }
}