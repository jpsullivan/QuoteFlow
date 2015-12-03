using System;
using System.Collections.Generic;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Statistics.Util;

namespace QuoteFlow.Api.Asset.Statistics
{
    public class IntegerFieldStatisticsMapper : IStatisticsMapper<int?>
    {
        public static readonly IStatisticsMapper<int?> Cost =
            new IntegerFieldStatisticsMapper(DocumentConstants.AssetCost);

        public string DocumentConstant { get; }

        public IntegerFieldStatisticsMapper(string documentConstant)
        {
            DocumentConstant = documentConstant;
        }
        
        public int? GetValueFromLuceneField(string documentValue)
        {
            if ("-1".Equals(documentValue))
            {
                return null;
            }

            return Convert.ToInt32(documentValue);
        }

        public IComparer<int?> Comparator => IntegerComparer.Comparator;

        public bool IsValidValue(int? value)
        {
            throw new System.NotImplementedException();
        }

        public bool FieldAlwaysPartOfAnAsset => true;

        public SearchRequest GetSearchUrlSuffix(int? value, SearchRequest searchRequest)
        {
            return null;
        }

        protected bool Equals(IntegerFieldStatisticsMapper other)
        {
            return string.Equals(DocumentConstant, other.DocumentConstant);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IntegerFieldStatisticsMapper) obj);
        }

        public override int GetHashCode()
        {
            return DocumentConstant?.GetHashCode() ?? 0;
        }
    }
}