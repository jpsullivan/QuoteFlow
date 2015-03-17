using System.ComponentModel;
using QuoteFlow.Api.Configuration.Lucene;

namespace QuoteFlow.Core.Configuration.Lucene
{
    /// <summary>
    /// Used to access the indexing configuration.
    /// </summary>
    public class IndexingConfiguration : IIndexingConfiguration
    {
        [DefaultValue(30000)]
        public int IndexLockWaitTime { get; set; }

        [DefaultValue(4000)]
        public int MaxReindexes { get; set; }

        [DefaultValue(400)]
        public int AssetsToForceOptimize { get; set; }

        [DefaultValue(true)]
        public bool IndexAvailable { get; set; }

        public void DisableIndex()
        {
            IndexAvailable = false;
        }

        public void EnableIndex()
        {
            IndexAvailable = true;
        }
    }
}