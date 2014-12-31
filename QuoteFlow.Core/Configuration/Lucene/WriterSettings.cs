using Lucene.Net.Analysis;

namespace QuoteFlow.Core.Configuration.Lucene
{
    public abstract class WriterSettings
    {
        private readonly ConfigurationService _config;

        public WriterSettings() { }

        protected WriterSettings(ConfigurationService config)
        {
            _config = config;
        }

        public virtual IndexWriterConfig GetWriterConfiguration(Analyzer analyser)
        {
            var mergePolicy = new TieredMergePolicy();
            mergePolicy.ExpungeDeletesPctAllowed = _config.IndexSettings.ExpungeDeletesPctAllowed;
            mergePolicy.FloorSegmentMB = _config.IndexSettings.FloorSegmentMB;
            mergePolicy.MaxMergedSegmentMB = _config.IndexSettings.MaxMergedSegmentMB;
            mergePolicy.MaxMergeAtOnce = _config.IndexSettings.MaxMergeAtOnce;
            mergePolicy.MaxMergeAtOnceExplicit = _config.IndexSettings.MaxMergeAtOnceExplicit;
            mergePolicy.NoCFSRatio = _config.IndexSettings.NoCfsRatio;
            mergePolicy.SegmentsPerTier = _config.IndexSettings.SegmentsPerTier;
            mergePolicy.UseCompoundFile = _config.IndexSettings.UseCompoundFile;

            IndexWriterConfig luceneConfig = new IndexWriterConfig(LuceneVersion.Get(), analyser);
            luceneConfig.MergePolicy = mergePolicy;
            luceneConfig.MaxBufferedDocs = MaxBufferedDocs;
            return luceneConfig;
        }

        public abstract int MaxBufferedDocs { get; }

        /// @deprecated Only applies to LogMergePolicy. 
        public abstract int MergeFactor { get; }

        /// @deprecated Only applies to LogMergePolicy. 
        public abstract int MaxMergeDocs { get; }

        /// @deprecated Not really relevant for Lucene 3.2+ . 
        public abstract int MaxFieldLength { get; }
    }
}