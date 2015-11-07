using Lucene.Net.Index;
using QuoteFlow.Core.Lucene.Index;

namespace QuoteFlow.Core.Configuration.Lucene
{
    /// <summary>
    /// Controls how the Lucene IndexWriter will be setup.
    /// </summary>
    public sealed class DefaultIndexWriterConfiguration
    {
        // use the Lucene IndexWriter default for this, as the default inside ILuceneConnection.DEFAULT_CONFIGURATION is HUGE!!!
        private const int MAX_FIELD_LENGTH = IndexWriter.DEFAULT_MAX_FIELD_LENGTH;

        public static readonly WriterSettings Batch = new BatchWriterSettings();
        private class BatchWriterSettings : WriterSettings
        {
            public BatchWriterSettings()
            {
            }

            public override int MergeFactor => 50;
            public override int MaxMergeDocs => int.MaxValue;
            public override int MaxBufferedDocs => 300;
            public override int MaxFieldLength => MAX_FIELD_LENGTH;
            public override IndexEngine.FlushPolicy FlushPolicy => IndexEngine.FlushPolicy.Flush;
            public override long CommitFrequency => 30000;
        }

        public static readonly WriterSettings Interactive = new InteractiveWriterSettings();
        private class InteractiveWriterSettings : WriterSettings
        {
            public InteractiveWriterSettings()
            {
            }

            public override int MergeFactor => 4;
            public override int MaxMergeDocs => 5000;
            public override int MaxBufferedDocs => 300;
            public override int MaxFieldLength => MAX_FIELD_LENGTH;
            public override IndexEngine.FlushPolicy FlushPolicy => IndexEngine.FlushPolicy.Flush;
            public override long CommitFrequency => 30000;
        }
    }
}