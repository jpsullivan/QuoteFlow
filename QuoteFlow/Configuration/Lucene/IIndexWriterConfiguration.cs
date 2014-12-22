namespace QuoteFlow.Configuration.Lucene
{
    /// <summary>
    /// Controls how the Lucene IndexWriter will be set up.
    /// </summary>
    public interface IIndexWriterConfiguration
    {
        DefaultIndexWriterConfiguration Default { get; set; }

        WriterSettings InterativeSettings { get; }

        WriterSettings BatchSettings { get; }
    }
}