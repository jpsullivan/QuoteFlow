using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Search.Searchers;

namespace QuoteFlow.Core.Asset.Search.Searchers
{
    /// <summary>
    /// Static Factory that can provide a Searcher Comparator for a given SearcherGroupType.
    /// </summary>
    public sealed class SearcherComparatorFactory
    {
        private static readonly IDictionary<SearcherGroupType, SearcherComparator> ComparatorMap;

		static SearcherComparatorFactory()
		{
			// Initialise
			var mapBuilder = new MapBuilder();

            mapBuilder.Add(SearcherGroupType.Text, typeof(TextQuerySearcher), typeof(SummaryQuerySearcher));
            mapBuilder.Add(SearcherGroupType.Context, typeof(CatalogSearcher), typeof(ManufacturerSearcher));
            mapBuilder.Add(SearcherGroupType.Catalog);
            mapBuilder.Add(SearcherGroupType.Asset, typeof(CostSearcher));
            mapBuilder.Add(SearcherGroupType.Date);

//			mapBuilder.Add(SearcherGroupType.Text, typeof(TextQuerySearcher), typeof(SummaryQuerySearcher), typeof(DescriptionQuerySearcher), typeof(EnvironmentQuerySearcher), typeof(CommentQuerySearcher));
//			mapBuilder.Add(SearcherGroupType.Context, typeof(CatalogSearcher), typeof(IssueTypeSearcher));
//			mapBuilder.Add(SearcherGroupType.Catalog, typeof(ComponentsSearcher), typeof(AffectedVersionsSearcher));
//			mapBuilder.Add(SearcherGroupType.Asset, typeof(ReporterSearcher), typeof(AssigneeSearcher), typeof(StatusSearcher), typeof(ResolutionSearcher), typeof(PrioritySearcher), typeof(LabelsSearcher));
//			mapBuilder.Add(SearcherGroupType.Date, typeof(CreatedDateSearcher), typeof(UpdatedDateSearcher), typeof(DueDateSearcher), typeof(ResolutionDateSearcher));
			mapBuilder.Add(SearcherGroupType.Custom);

			ComparatorMap = mapBuilder.ToImmutableMap();
		}

		public static IComparer<IAssetSearcher<ISearchableField>> GetSearcherComparator(SearcherGroupType searcherGroupType)
		{
			return ComparatorMap[searcherGroupType];
		}

		private sealed class MapBuilder
		{
		    private readonly Dictionary<SearcherGroupType, SearcherComparator> _map = new Dictionary<SearcherGroupType, SearcherComparator>();

			public void Add(SearcherGroupType searcherGroupType, params Type[] classes)
			{
				_map[searcherGroupType] = new SearcherComparator(classes.ToList());
			}

			public IDictionary<SearcherGroupType, SearcherComparator> ToImmutableMap()
			{
				return new Dictionary<SearcherGroupType, SearcherComparator>(_map);
			}
		}

        private sealed class SearcherComparator : IComparer<IAssetSearcher<ISearchableField>>
		{
            private readonly IList<Type> _orderList;

			internal SearcherComparator(IList<Type> orderList)
			{
				_orderList = orderList;
			}

            public int Compare(IAssetSearcher<ISearchableField> o1, IAssetSearcher<ISearchableField> o2)
			{
				int o1position = IndexOf(o1);
				int o2position = IndexOf(o2);

				if (o1position == -1)
				{
				    if (o2position == -1)
					{
						return 0;
					}
				    return 1;
				}
			    if (o2position == -1)
			    {
			        return -1;
			    }
			    return o1position - o2position;
			}

            private int IndexOf(IAssetSearcher<ISearchableField> searcher)
			{
				return _orderList.IndexOf(searcher.GetType());
			}
		}
    }
}