using System;
using Lucene.Net.Analysis;
using Lucene.Net.Store;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Core.Configuration.Lucene;

namespace QuoteFlow.Core.Lucene.Index
{
    public class IndexConfiguration : IIndexConfiguration
    {
        public sealed class Default
        {
            /// <summary>
            /// 1million (the lucene default is 10,000).
            /// at (say) 10chars per token, that is a 10meg limit. Fair enough.
            /// </summary>
            private const int MAX_FIELD_LENGTH = 1000000;

            public static readonly WriterSettings Interactive = new InteractiveWriterSettings();
            private class InteractiveWriterSettings : WriterSettings
            {
                public override int MergeFactor
                {
                    get { return 4; }
                }

                public override int MaxMergeDocs
                {
                    get { return 5000; }
                }

                public override int MaxBufferedDocs
                {
                    get { return 300; }
                }

                public override int MaxFieldLength
                {
                    get { return MAX_FIELD_LENGTH; }
                }
            }

            public static readonly WriterSettings Batch = new BatchWriterSettings();
            private class BatchWriterSettings : WriterSettings
            {
                public override int MergeFactor
                {
                    get { return 50; }
                }

                public override int MaxMergeDocs
                {
                    get { return Int32.MaxValue; }
                }

                public override int MaxBufferedDocs
                {
                    get { return 300; }
                }

                public override int MaxFieldLength
                {
                    get { return MAX_FIELD_LENGTH; }
                }
            }

            public static readonly IndexWriterConfiguration WriterConfiguration = new IndexWriterConfiguration();
            public class IndexWriterConfiguration : IIndexWriterConfiguration
            {
                public DefaultIndexWriterConfiguration Default { get; set; }

                public WriterSettings InterativeSettings
                {
                    get { return Interactive; }
                }

                public WriterSettings BatchSettings
                {
                    get { return Batch; }
                }
            }
        }

        public Directory Directory { get; set; }
        public Analyzer Analyzer { get; set; }
        public IIndexWriterConfiguration WriterConfiguration { get; set; }

        public IndexConfiguration(Directory directory, Analyzer analyzer) : this(directory, analyzer, Default.WriterConfiguration)
        {
        }

        public IndexConfiguration(Directory directory, Analyzer analyzer, IIndexWriterConfiguration writerConfiguration)
        {
            Directory = directory;
            Analyzer = analyzer;
            WriterConfiguration = writerConfiguration;
        }

        public WriterSettings GetWriterSettings(UpdateMode mode)
        {
            switch (mode)
            {
                case UpdateMode.Batch:
                    return WriterConfiguration.BatchSettings;
                case UpdateMode.Interactive:
                    return WriterConfiguration.InterativeSettings;
            }
            return null;
        }
    }
}
