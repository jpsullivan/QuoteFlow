using System;
using System.Collections.Generic;

namespace QuoteFlow.Api.Asset.Statistics.Util
{
    public class IntegerComparer : IComparer<int?>
    {
        public static readonly IComparer<int?> Comparator = new IntegerComparer();
        
        private IntegerComparer()
        {
            // use static instance
        } 

        public int Compare(int? x, int? y)
        {
            if (x == null && y == null)
            {
                return 0;
            }
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }

            int x2 = x.Value;
            int y2 = y.Value;

            return x2.CompareTo(y2);
        }
    }
}