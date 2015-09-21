namespace QuoteFlow.Api.Asset.Index
{
    /// <summary>
	/// Provides parameters required to conduct indexing or re-indexing QuoteFlow assets.
	/// 
	/// All parameters except indexAssets are by default set to false.
	/// 
	/// Use <see cref="IndexNone"/> to set all values to false.
	/// Use <see cref="IndexAssetOnly"/> to set all values to false except <see cref="IndexAssets"/>.
	/// Use <see cref="Index_All"/> to set all values to true.
	/// 
	/// - indexAssets - true if assets should be indexed
	/// - indexComments - true if comments should be indexed
	/// 
	/// Clients should use the provided <see cref="Builder"/> to construct 
	/// an instance of this class.
	/// </summary>
    public class AssetIndexingParams
    {
        public static AssetIndexingParams IndexNone = new AssetIndexingParams(false, false);
        public static AssetIndexingParams IndexAssetOnly = new AssetIndexingParams(true, false);
        public static AssetIndexingParams Index_All = new AssetIndexingParams(true, true);

        private AssetIndexingParams(bool indexAssets, bool indexComments)
        {
            IndexAssets = indexAssets;
            IndexComments = indexComments;
        }

        public static ParamsBuilder Builder()
        {
            return new ParamsBuilder();
        }

        public bool IndexAssets { get; }

        public bool IndexComments { get; }

        public bool Index => IndexAssets || IndexComments;

        public bool IndexAll => IndexAssets && IndexComments;

        public override string ToString()
        {
            return $"{{indexAssets={IndexAssets}, indexComments={IndexComments}}}";
        }

        protected bool Equals(AssetIndexingParams other)
        {
            return IndexAssets == other.IndexAssets && IndexComments == other.IndexComments;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssetIndexingParams) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (IndexAssets.GetHashCode()*397) ^ IndexComments.GetHashCode();
            }
        }

        public class ParamsBuilder
        {
            private bool IndexAssets = true;
            private bool IndexComments = false;

            /// <summary>
            /// Creates builder with ORs on each param.
            /// </summary>
            public ParamsBuilder AddIndexingParams(AssetIndexingParams assetIndexingParams)
            {
                IndexAssets |= assetIndexingParams.IndexAssets;
                IndexComments |= assetIndexingParams.IndexComments;
                return this;
            }

            public virtual ParamsBuilder SetAssets(bool indexIssues)
            {
                IndexAssets = indexIssues;
                return this;
            }

            public virtual ParamsBuilder SetComments(bool indexComments)
            {
                IndexComments = indexComments;
                return this;
            }

            public virtual ParamsBuilder WithComments()
            {
                IndexComments = true;
                return this;
            }

            public virtual ParamsBuilder WithoutAssets()
            {
                IndexAssets = false;
                return this;
            }

            public virtual AssetIndexingParams Build()
            {
                return new AssetIndexingParams(IndexAssets, IndexComments);
            }
        }
    }
}
