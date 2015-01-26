using System.Collections.Generic;

namespace QuoteFlow.Api.Jql.Query
{
    public class FilteredSearcherGroup
    {
        /// <summary>
        /// 
        /// </summary>
        private List<Searcher> Searchers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private string Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private string Title { get; set; }

        public FilteredSearcherGroup() { }

        public FilteredSearcherGroup(string type)
        {
            Type = type;
            Title = null;
            Searchers = new List<Searcher>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searcher"></param>
        public void AddSearcher(Searcher searcher)
        {
            Searchers.Add(searcher);
        }
    }
}