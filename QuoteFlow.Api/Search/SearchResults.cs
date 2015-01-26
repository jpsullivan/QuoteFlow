using System;
using System.Collections.Generic;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Infrastructure.Paging;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Search
{
    public class SearchResults : IPagerFilter
    {
        /// <summary>
        /// A collection of <see cref="AssetPage"/> objects
        /// </summary>
        private ICollection<Page> pages;
        private const int PAGES_TO_LIST = 5;

        private int start;
        private readonly int max;
        private readonly int total;
        private readonly IList<IAsset> assets;

        /// <summary>
        /// Construct searchResults using a list of assets.  The assets returned by <seealso cref="#getIssues()"/> will
        /// be a subset of the assets passed in.
        /// </summary>
        /// <param name="assets">A list of <see cref="Asset"/> objects.</param>
        /// <param name="pagerFilter">Representing which assets to limit the results to.</param>
        public SearchResults(List<IAsset> assets, PagerFilter<IAsset> pagerFilter)
        {
            // Reset the pager filters start value if the current value is not sane
            if (assets.Count < pagerFilter.Start)
            {
                pagerFilter.Start = 0;
            }
            start = pagerFilter.Start;
            total = assets.Count;
            max = pagerFilter.Max;
            this.assets = pagerFilter.GetCurrentPage(assets);
        }

        /// <summary>
        /// Construct searchResults using the assets that should be displayed, and the 'total' number of assets.
        /// This is used when a search does not wish to load the entire list of assets into memory.
        /// </summary>
        /// <param name="assetsInPage">A list of <see cref="Asset"/> objects.</param>
        /// <param name="totalIssueCount">The count of the number of assets returned.</param>
        /// <param name="pagerFilter">Representing the users preference for paging.</param>
        public SearchResults(IList<IAsset> assetsInPage, int totalIssueCount, IPagerFilter pagerFilter)
        {
            // Reset the pager filters start value if the current value is not sane
            if (totalIssueCount < pagerFilter.Start)
            {
                pagerFilter.Start = 0;
            }
            start = pagerFilter.Start;
            total = totalIssueCount;
            max = pagerFilter.Max;
            assets = assetsInPage;
        }

        /// <summary>
        /// Construct searchResults using the assets that should be displayed, and the 'total' number of assets.
        /// This is used when we do a stable search and want to return a max of the selected page's length, not the
        /// stable search limit.
        /// </summary>
        /// <param name="assetsInPage">A list of <seealso cref="Asset"/> objects.</param>
        /// <param name="totalIssueCount">The count of the number of assets returned.</param>
        /// <param name="maxIssueCount">The maximum number of assets to include in the search.</param>
        /// <param name="startIndex">The index of the first issue in the search.</param>
        public SearchResults(IList<IAsset> assetsInPage, int totalIssueCount, int maxIssueCount, int startIndex)
        {
            // Reset the pager filters start value if the current value is not sane
            if (totalIssueCount < startIndex)
            {
                startIndex = 0;
            }
            start = startIndex;
            total = totalIssueCount;
            max = maxIssueCount;
            assets = assetsInPage;
        }

        /// <summary>
        /// Get the assets available in this page.
        /// </summary>
        /// <returns> A list of <see cref="IAsset"/> objects.</returns>
        public virtual IEnumerable<IAsset> Assets
        {
            get { return assets; }
        }

        public virtual int Start
        {
            get { return start; }
            set { start = value; }
        }

        public virtual int End
        {
            get { return Math.Min(start + max, total); }
        }

        public int Max { get { return total; } set {} }

        public virtual int Total
        {
            get { return total; }
        }

        public int PageSize { get; private set; }

        public virtual int NextStart
        {
            get { return start + max; }
        }

        public virtual int PreviousStart
        {
            get { return Math.Max(0, start - max); }
        }

        /// <summary>
        /// Return the 'readable' start (ie 1 instead of 0)
        /// </summary>
        public virtual int NiceStart
        {
            get { return Assets.AnySafe() ? 0 : Start + 1; }
        }

        public virtual IList<Page> Pages
        {
            get
            {
                if (pages == null)
                {
                    pages = GeneratePages();
                }

                return RestrictPages(pages, total);
            }
        }

        /// <summary>
        /// Generates a collection of page objects which keep track of the pages for display
        /// </summary>
        internal virtual IList<Page> GeneratePages()
        {
            if (total == 0)
            {
                return new List<Page>();
            }
            if (max <= 0)
            {
                throw new System.ArgumentException("Issue per page should be 1 or greater.");
            }

            IList<Page> pages = new List<Page>();

            int pageNumber = 1;
            for (int index = 0; index < total; index += max)
            {
                bool isCurrentPage = (start >= index) && (start < index + max);
                pages.Add(new Page(index, pageNumber, isCurrentPage));
                pageNumber++;
            }

            return pages;
        }

        /// <summary>
        /// Restrict the pagers to a certain number of pages on either side of the current page.
        /// <p/>
        /// The number of pages to list is stored in <seealso cref="#PAGES_TO_LIST"/>.
        /// </summary>
        internal virtual IList<Page> RestrictPages(IEnumerable<Page> pages, int size)
        {
            IList<Page> pagesToDisplay = new List<Page>(2 * PAGES_TO_LIST);

            // enhance the calculation so that at least
            // PAGES_TO_LIST-1 pages are always shown
            //
            // calculate sliding window
            int maxpage = (size + max - 1) / max; // 1 .. n
            int firstpage = 1; // 1 .. n
            int lastpage = firstpage + PAGES_TO_LIST + PAGES_TO_LIST - 2; // 1 .. n
            if (lastpage < maxpage)
            {
                int ourpage = (Start / max) + 1; // 1 .. n
                if (ourpage - firstpage > PAGES_TO_LIST - 1)
                {
                    lastpage = ourpage + PAGES_TO_LIST - 1;
                    if (lastpage > maxpage)
                    {
                        lastpage = maxpage;
                    }
                    firstpage = lastpage - PAGES_TO_LIST - PAGES_TO_LIST + 2;
                }
            }
            else if (lastpage > maxpage)
            {
                lastpage = maxpage;
            }

            int minstart = (firstpage - 1) * max;
            int maxstart = (lastpage - 1) * max;
            foreach (Page page in pages)
            {
                if (page.Start > size) continue;

                bool largerThanMin = page.Start >= minstart;
                bool smallerThanMax = page.Start <= maxstart;
                if (largerThanMin && smallerThanMax)
                {
                    pagesToDisplay.Add(page);
                }
            }
            return pagesToDisplay;
        }

        public sealed class Page : IAssetPage
        {
            public int Start { get; set; }
            public int PageNumber { get; set; }

            private readonly bool _currentPage;

            public Page(int start, int pageNumber, bool isCurrentPage)
            {
                Start = start;
                PageNumber = pageNumber;
                _currentPage = isCurrentPage;
            }

            public bool IsCurrentPage()
            {
                return _currentPage;
            }
        }
    }

}