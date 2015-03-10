namespace QuoteFlow.Api.Configuration.Lucene
{
    /// <summary>
    /// Responsible for determining the current location of QuoteFlow indexes.
    /// </summary>
    public interface IIndexPathManager
    {
        /// <summary>
        /// Returns the root path of QuoteFlow's indexes.
        /// If QuoteFlow is configured to "Use the Default Index Directory", then the absolute path 
        /// of that default directory is returned.
        /// </summary>
        /// <returns> the root path of QuoteFlow's indexes </returns>
        string IndexRootPath { get; set; }

        /// <summary>
        /// This returns the root index directory that QuoteFlow will use by default, if it is 
        /// configured to do so.
        /// </summary>
        /// <returns>The default root index path</returns>
        string DefaultIndexRootPath { get; }

        /// <summary>
        /// Returns the path of QuoteFlow's asset indexes. </summary>
        /// <returns> the path of QuoteFlow's asset indexes </returns>
        string AssetIndexPath { get; }

        /// <summary>
        /// Returns the path of QuoteFlow's comment indexes. </summary>
        /// <returns> the path of QuoteFlow's comment indexes </returns>
        string CommentIndexPath { get; }

        IndexPathManagerMode Mode { get; }
    }

    public enum IndexPathManagerMode
    {
        Default,
        Custom,
        Disabled
    }
}