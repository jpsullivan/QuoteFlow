using System;

namespace QuoteFlow.Core.Lucene.Index
{
    /// <summary>
    /// Exception indicating some errors occurred during the indexing process.
    /// </summary>
    [Serializable]
    public class IndexingFailureException : Exception
    {
        private readonly int _failures;

        public IndexingFailureException(int failures)
        {
            _failures = failures;
        }

        public override string Message => $"Indexing completed with {_failures:D} errors";
    }
}
