using System.ComponentModel;

namespace QuoteFlow.Core.Configuration
{
    public class IndexConfiguration
    {
        [DefaultValue(10)]
        [Description("")]
        public int ExpungeDeletesPctAllowed { get; set; }

        [DefaultValue(2)]
        [Description("")]
        public int FloorSegmentMB { get; set; }

        [DefaultValue(512)]
        public int MaxMergedSegmentMB { get; set; }

        [DefaultValue(10)]
        public int MaxMergeAtOnce { get; set; }

        [DefaultValue(30)]
        public int MaxMergeAtOnceExplicit { get; set; }

        [DefaultValue(10)]
        public int NoCfsRatio { get; set; }

        [DefaultValue(10)]
        public int SegmentsPerTier { get; set; }

        [DefaultValue(true)]
        public bool UseCompoundFile { get; set; }
    }
}