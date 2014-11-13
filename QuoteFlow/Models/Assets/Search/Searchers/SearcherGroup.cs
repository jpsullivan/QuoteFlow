using System.Collections.Generic;
using QuoteFlow.Models.Assets.Fields;

namespace QuoteFlow.Models.Assets.Search.Searchers
{
    public class SearcherGroup
    {
        public SearcherGroupType Type { get; set; }
        public IEnumerable<IAssetSearcher<ISearchableField>> Searchers { get; set; } 
        public bool PrintHeader { get; set; }

        public SearcherGroup(SearcherGroupType type, IEnumerable<IAssetSearcher<ISearchableField>> searchers)
        {
            Type = type;
            Searchers = searchers;
            PrintHeader = false;
        }

        public override string ToString()
        {
            return string.Format("Searcher Group: [Type: {0}, Searchers: {1}].", Type, Searchers);
        }
    }
}