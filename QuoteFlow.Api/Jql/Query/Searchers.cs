using System.Collections.Generic;
using Jil;

namespace QuoteFlow.Api.Jql.Query
{
    public class Searchers
    {
        /// <summary>
        /// 
        /// </summary>
        [JilDirective("groups")]
        public List<FilteredSearcherGroup> Groups { get; set; }

        public Searchers()
        {
            Groups = new List<FilteredSearcherGroup>();
        }

        public void AddGroup(FilteredSearcherGroup group)
        {
            Groups.Add(group);
        }

        public void AddGroups(IEnumerable<FilteredSearcherGroup> groups)
        {
            Groups.AddRange(groups);
        }
    }
}