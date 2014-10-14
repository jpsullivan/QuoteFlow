using System.Collections.Generic;

namespace QuoteFlow.Models.Search.Jql.Query
{
    public class Searchers
    {
        /// <summary>
        /// 
        /// </summary>
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