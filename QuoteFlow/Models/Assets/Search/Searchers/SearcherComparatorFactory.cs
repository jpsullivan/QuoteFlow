using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Models.Assets.Fields;

namespace QuoteFlow.Models.Assets.Search.Searchers
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

			mapBuilder.Add(SearcherGroupType.Text, typeof(TextQuerySearcher), typeof(SummaryQuerySearcher), typeof(DescriptionQuerySearcher), typeof(EnvironmentQuerySearcher), typeof(CommentQuerySearcher));
			mapBuilder.Add(SearcherGroupType.Context, typeof(ProjectSearcher), typeof(IssueTypeSearcher));
			mapBuilder.Add(SearcherGroupType.Catalog, typeof(FixForVersionsSearcher), typeof(ComponentsSearcher), typeof(AffectedVersionsSearcher));
			mapBuilder.Add(SearcherGroupType.Asset, typeof(ReporterSearcher), typeof(AssigneeSearcher), typeof(StatusSearcher), typeof(ResolutionSearcher), typeof(PrioritySearcher), typeof(LabelsSearcher));
			mapBuilder.Add(SearcherGroupType.Date, typeof(CreatedDateSearcher), typeof(UpdatedDateSearcher), typeof(DueDateSearcher), typeof(ResolutionDateSearcher));
			mapBuilder.Add(SearcherGroupType.Custom);

			ComparatorMap = mapBuilder.ToImmutableMap();
		}

		public static IComparer<IAssetSearcher<ISearchableField>> GetSearcherComparator(SearcherGroupType searcherGroupType)
		{
			return ComparatorMap[searcherGroupType];
		}

		private class MapBuilder
		{
			internal readonly Dictionary<SearcherGroupType, SearcherComparator> Map = new Dictionary<SearcherGroupType, SearcherComparator>();

			public virtual void Add(SearcherGroupType searcherGroupType, params Type[] classes)
			{
				Map[searcherGroupType] = new SearcherComparator(classes.ToList());
			}

			public virtual IDictionary<SearcherGroupType, SearcherComparator> ToImmutableMap()
			{
				return new Dictionary<SearcherGroupType, SearcherComparator>(Map);
			}
		}

        internal sealed class SearcherComparator : IComparer<IAssetSearcher<ISearchableField>>
		{
			internal readonly IList<Type> orderList;

			internal SearcherComparator(IList<Type> orderList)
			{
				this.orderList = orderList;
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

			internal int IndexOf(IAssetSearcher<ISearchableField> searcher)
			{
				return orderList.IndexOf(searcher.GetType());
			}
		}

    }
}