using System.Collections.Generic;
using QuoteFlow.Api.Asset.Search;

namespace QuoteFlow.Api.Asset.Statistics
{
    public abstract class AbstractConstantStatisticsMapper : IStatisticsMapper<IAssetConstant>
    {
        public abstract string DocumentConstant { get; }

        protected internal abstract string ConstantType { get; }

        protected internal abstract string AssetFieldConstant { get; }
        
        public bool FieldAlwaysPartOfAnAsset => true;

        public IAssetConstant GetValueFromLuceneField(string documentValue)
        {
            throw new System.NotImplementedException();
        }

        public IComparer<IAssetConstant> Comparator { get; }

        public bool IsValidValue(IAssetConstant value)
        {
            return true;
        }

        public SearchRequest GetSearchUrlSuffix(IAssetConstant value, SearchRequest searchRequest)
        {
            throw new System.NotImplementedException();
        }
    }
}