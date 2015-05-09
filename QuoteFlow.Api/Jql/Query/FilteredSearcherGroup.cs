using System.Collections.Generic;
using Jil;

namespace QuoteFlow.Api.Jql.Query
{
    public class FilteredSearcherGroup
    {
        public FilteredSearcherGroup()
        {
        }

        public FilteredSearcherGroup(string type)
        {
            Type = type;
            Title = null;
            Searchers = new List<Searcher>();
        }

        /// <summary>
        /// 
        /// </summary>
        [JilDirective("searchers")]
        public IList<Searcher> Searchers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JilDirective("type")]
        public string Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JilDirective("title")]
        public string Title { get; set; }

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