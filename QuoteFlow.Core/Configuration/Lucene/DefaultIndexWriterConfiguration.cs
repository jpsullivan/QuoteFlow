using Lucene.Net.Index;

namespace QuoteFlow.Core.Configuration.Lucene
{
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

            public override int MergeFactor
            {
                get { return 50; }
            }

            public override int MaxMergeDocs
            {
                get { return int.MaxValue; }
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

        public static readonly WriterSettings Interactive = new InteractiveWriterSettings();
        private class InteractiveWriterSettings : WriterSettings
        {
            public InteractiveWriterSettings()
            {
            }

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
    }
}