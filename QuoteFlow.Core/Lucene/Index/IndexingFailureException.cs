using System;

namespace QuoteFlow.Core.Lucene.Index
{
    /// <summary>
    /// Exception indicating some errors occurred during the indexing process.
    /// </summary>
    [Serializable]
    public class IndexingFailureException : Exception
    {
        private readonly int failures;

        public IndexingFailureException(int failures)
        {
            this.failures = failures;
        }

        public override string Message
        {
            get { return string.Format("Indexing completed with {0:D} errors", failures); }
        }
    }
}
