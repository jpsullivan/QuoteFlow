using System;

namespace QuoteFlow.Api.Infrastructure.Paging
{
    [Serializable]
    public class AssetPage
    {
        public int Start { get; set; }
        public IPagerFilter PagerFilter { get; set; }
        public int PageNumber { get; set; }

        public AssetPage(int start, int pageNumber, IPagerFilter pagerFilter)
        {
            Start = start;
            PageNumber = pageNumber;
            PagerFilter = pagerFilter;
        }

        public bool IsCurrentPage()
        {
            return PagerFilter.Start >= Start && PagerFilter.Start < (Start + PagerFilter.Max);
        }
    }
}