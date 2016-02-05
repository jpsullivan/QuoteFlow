using System;
using System.Collections.Generic;
using QuoteFlow.Api.Asset.Comparator;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Search;

namespace QuoteFlow.Core.Asset.Statistics
{
//    public class AssetIdStatisticsMapper : ILuceneFieldSorter<int>
//    {
//        public static readonly ILuceneFieldSorter<int> Mapper = new AssetIdStatisticsMapper();
//        
//        public string DocumentConstant => DocumentConstants.AssetId;
//
//        public IComparer<int> Comparator => IdComparator.Comparator;
//
//        public int GetValueFromLuceneField(string documentValue)
//        {
//            return Convert.ToInt32(documentValue);
//        }
//
//        public override bool Equals(object obj)
//        {
//            if (this == obj)
//            {
//                return true;
//            }
//
//            if (obj == null || GetType() != obj.GetType())
//            {
//                return false;
//            }
//
//            AssetIdStatisticsMapper that = (AssetIdStatisticsMapper) obj;
//            return DocumentConstants.AssetId.Equals(that.DocumentConstant);
//        }
//    }
}