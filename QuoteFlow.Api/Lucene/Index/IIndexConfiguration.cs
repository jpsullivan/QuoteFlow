using Lucene.Net.Analysis;
using Lucene.Net.Store;

namespace QuoteFlow.Api.Lucene.Index
{
    /// <summary>
    /// The configuration for a particular index and how it should be written.
    /// </summary>
    public interface IIndexConfiguration
    {
        Directory Directory { get; set; }
        Analyzer Analyzer { get; set; }
    }
}