using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Models.Assets.Fields;

namespace QuoteFlow.Models.Search.Searchers
{
    public class SearcherGroup
    {
        public SearcherGroupType Type { get; set; }
        public IEnumerable<IAssetSearcher<ISearchableField>> Searchers { get; set; } 
        public bool PrintHeader { get; set; }

        public SearcherGroup(SearcherGroupType type, IEnumerable<IAssetSearcher<ISearchableField>> searchers, bool printHeader)
        {
            Type = type;
            Searchers = searchers;
            PrintHeader = printHeader;
        }

        public bool IsShown(User searcher, SearchContext searchContext)
        {
            if (searchContext == null)
            {
                throw new ArgumentNullException("searchContext");
            }

            foreach (var assetSearcher in Searchers)
            {
                if (assetSearcher.SearchRenderer().IsShown(searcher, searchContext))
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("Searcher Group: [Type: {0}, Searchers: {1}].", Type, Searchers);
        }
    }
}