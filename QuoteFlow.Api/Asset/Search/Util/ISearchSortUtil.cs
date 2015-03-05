using System.Collections;
using System.Collections.Generic;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Order;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset.Search.Util
{
    /// <summary>
    /// Looks at the current search sorts on a query and will add the default QuoteFlow search sorts (asset id or none if there
    /// is a text search included in the query) if there are no user specified sorts.
    /// </summary>
    public interface ISearchSortUtil
    {
        string SorterOrder { get; }
        string SorterField { get; }

        /// <summary>
        /// Combine the new search sorts and the old search sorts returning a list of sorts that is only of size maxLength.
        /// Old sorts will fall off the end of the list first. If there are any sorts that are duplicated (the field is
        /// mentioned again, sort order not taken into account), then the old sort reference will not be mentioned and will
        /// be replaced with the new sort in the correct position in the list.
        /// </summary>
        /// <param name="user">Performing the search.</param>
        /// <param name="newSorts">The new sorts that should go in the front of the sort list; must not be null.</param>
        /// <param name="oldSorts">The old sorts that should be in the end of the sort list; may be null.</param>
        /// <param name="maxLength">The max size of the produced list.</param>
        /// <returns>A list of search sorts that contains the newest and oldest sorts respecting the max length.</returns>
        IList<SearchSort> MergeSearchSorts(User user, ICollection<SearchSort> newSorts, ICollection<SearchSort> oldSorts, int maxLength);

        IEnumerable<SearchSort> GetSearchSorts(IQuery query);

        /// <summary>
        /// This method is used to convert incomming, request-style, parameters into SearchSort objects.
        /// </summary>
        /// <param name="parameterMap">
        /// Contains 0 or many "sorter/order" and "sorter/field" parameters that will 
        /// be converted into a search sort. The field is the System/Custom field name and will be converted by 
        /// this method into the JQL Primary clause name. The reason for this is that we need to support 
        /// "old (pre 4.0)" URL parameters and these contain the field id, not the clause name. Since the UI is 
        /// the only thing producing these parameters we decided to leave it generating the field id. When sorts 
        /// are specified in JQL they will be in clause names.
        /// </param>
        /// <returns>
        /// An OrderBy that can be used to populate a <see cref="IQuery"/> which contains a list
        /// of SearchSort's that relate to the passed in parameters. Will be an order by with empty sorts if there are no
        /// search sorts in the parameters.
        /// </returns>
        IOrderBy GetOrderByClause(IDictionary parameterMap);

        /// <summary>
        /// Concatenate the new search sorts and the old search sorts returning a list of sorts that is only of size maxLength.
        /// Old sorts will fall off the end of the list first.
        /// </summary>
        /// <param name="newSorts">The new sorts that should go in the front of the sort list; must not be null.</param>
        /// <param name="oldSorts">The old sorts that should be in the end of the sort list; may be null.</param>
        /// <param name="maxLength">The max size of the produced list.</param>
        /// <returns>A list of search sorts that contains the newest and oldest sorts respecting the max length.</returns>
        IList<SearchSort> ConcatSearchSorts(IEnumerable<SearchSort> newSorts, IEnumerable<SearchSort> oldSorts, int maxLength);

        /// <summary>
        /// Returns a list of the descriptions of each sorter defined in the search request. 
        /// If one of the sorters references a field which does not exist, it will be skipped.
        /// </summary>
        /// <param name="searchRequest">The search request containing the sorts; must not be null.</param>
        /// <param name="searcher"> the user making the request </param>
        /// <returns>A list of strings describing the sorters; never null.</returns>
        IEnumerable<string> GetSearchSortDescriptions(SearchRequest searchRequest, User searcher);

    }

    public static class SearchSorters
    {
        public const string Order = "sorter/order";
        public const string Field = "sorter/field";
    }
}