namespace QuoteFlow.Api.Configuration.Lucene
{
    /// <summary>
    /// Used to access the indexing configuration.
    /// </summary>
    public interface IIndexingConfiguration
    {
        int IndexLockWaitTime { get; set; }

        int MaxReindexes { get; set; }

        int AssetsToForceOptimize { get; set; }

        bool IndexAvailable { get; set; }

        void DisableIndex();

        void EnableIndex(); 
    }
}