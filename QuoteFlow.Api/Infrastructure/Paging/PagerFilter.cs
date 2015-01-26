using System;
using System.Collections.Generic;

namespace QuoteFlow.Api.Infrastructure.Paging
{
    /// <summary>
    /// This is a super class that implements paging for browsers.
    /// Most other filters (which want paging ability) will extend this.
    /// </summary>
    [Serializable]
    public class PagerFilter<T> : IPagerFilter
    {
        private const int PAGES_TO_LIST = 5;

        // the number of issues per page
        private int max = 20;
        private int start;

        /// <summary>
        /// A collection of <seealso cref="AssetPage"/> objects
        /// </summary>
        /// @deprecated since 4.0 use #getPages() rather than access pages directly 
        protected internal List<AssetPage> pages;

        public PagerFilter()
        {
        }

        public PagerFilter(PagerFilter<T> old)
        {
            this.Max = old.Max;
            this.Start = old.Start;
        }

        public PagerFilter(int max)
        {
            this.max = max == -1 ? int.MaxValue : max;
        }

        public PagerFilter(int start, int max)
            : this(max)
        {
            Start = start;
        }

        /// <summary>
        /// A pager that will return unlimited number of objects.
        /// </summary>
        /// <returns> A PagerFilter with a max set to <seealso cref="Int32.MaxValue"/> </returns>
        public static PagerFilter<T> UnlimitedFilter
        {
            get { return new PagerFilter<T>(int.MaxValue); }
        }

        /// <summary>
        /// A pager that has its start aligned to the page containing the index.
        /// </summary>
        /// <param name="index"> the index of a result whose page you want the pager to start at </param>
        /// <param name="max"> the maximum number of results in a page </param>
        /// <returns> a new pager aligned to the page containing the index </returns>
        /// <exception cref="IllegalArgumentException"> if index is less than 0 </exception>
        public static PagerFilter<T> NewPageAlignedFilter(int index, int max)
        {
            if (index < 0)
            {
                throw new ArgumentException(string.Format("index {0:D} is less than 0.", index));
            }
            
            if (max == 0)
            {
                return new PagerFilter<T>(index, max);
            }

            return new PagerFilter<T>(index - (index % max), max);
        }

        /// <summary>
        /// Gets the current page out of a list of objects.
        /// </summary>
        /// <returns> the sublist that is the current page. </returns>
        public virtual IList<T> GetCurrentPage(List<T> itemsCol)
        {
            List<T> items = itemsCol ?? new List<T>();

            if (items.Count == 0)
            {
                start = 0;
                return new List<T>();
            }

            // now return the appropriate page of issues
            // now make sure that the start is valid
            if (start >= items.Count)
            {
                start = 0;
                return items.GetRange(0, Math.Min(max, items.Count));
            }

            return items.GetRange(start, Math.Min(start + max, items.Count));
        }

        public virtual IList<AssetPage> GetPages(ICollection<T> itemsCol)
        {
            if (pages == null)
            {
                pages = GeneratePages(itemsCol);
            }

            return RestrictPages(pages, itemsCol.Count);
        }

        /// <summary>
        /// generates a collection of page objects which keep track of the pages for display
        /// </summary>
        /// <param name="items"> </param>
        public virtual List<AssetPage> GeneratePages(ICollection<T> items)
        {
            if ((items == null) || items.Count == 0)
            {
                return new List<AssetPage>();
            }

            IList<AssetPage> pages = new List<AssetPage>();
            int total = items.Count;

            int pageNumber = 1;
            for (int index = 0; index < total; index += max)
            {
                pages.Add(new AssetPage(index, pageNumber, this));
                pageNumber++;
            }

            return new List<AssetPage>(pages);
        }

        /// <summary>
        /// Restrict the pagers to a certain number of pages on either side of the current page.
        /// <p/>
        /// The number of pages to list is stored in <seealso cref="#PAGES_TO_LIST"/>.
        /// </summary>
        public virtual IList<AssetPage> RestrictPages(List<AssetPage> assetPages, int size)
        {
            IList<AssetPage> pagesToDisplay = new List<AssetPage>(2 * PAGES_TO_LIST);

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
            foreach (AssetPage page in assetPages)
            {
                if (page.Start > size) continue;

                bool largerThanMin = page.Start >= minstart;
                bool smallerThanMax = page.Start <= maxstart;
                if (largerThanMin && smallerThanMax)
                {
                    pagesToDisplay.Add(page);
                }
            }
            return new List<AssetPage>(assetPages);
        }

        public virtual int Max
        {
            get
            {
                return max;
            }
            set
            {
                if (this.max != value)
                {
                    pages = null;
                }
                this.max = value;
            }
        }

        public virtual int PageSize
        {
            get
            {
                return Max;
            }
        }

        public virtual int Start
        {
            get
            {
                return start;
            }
            set
            {
                this.start = value;
            }
        }

        public virtual int End
        {
            get
            {
                return Math.Max(start + max, max);
            }
        }

        public virtual int NextStart
        {
            get
            {
                return Math.Max(start + max, max);
            }
        }

        public virtual int PreviousStart
        {
            get
            {
                return Math.Max(0, start - max);
            }
        }

        public override string ToString()
        {
            return string.Format("start: {0}, end: {1}, max: {2}", Start.ToString(), End.ToString(), Max.ToString());
        }
    }
}