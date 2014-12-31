using Lucene.Net.Analysis;
using Lucene.Net.Store;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Core.Configuration.Lucene;

namespace QuoteFlow.Core.Lucene.Index
{
    /// <summary>
    /// The configuration for a particular index and how it should be written.
    /// </summary>
    public interface IIndexConfiguration
    {
        Directory Directory { get; set; }

        Analyzer Analyzer { get; set; }

        WriterSettings GetWriterSettings(UpdateMode mode);
    }
}