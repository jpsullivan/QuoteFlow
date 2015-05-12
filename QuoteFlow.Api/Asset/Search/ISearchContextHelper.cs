using QuoteFlow.Api.Jql.Query;

namespace QuoteFlow.Api.Asset.Search
{
    /// <summary>
    /// Utilities for generating search context.
    /// </summary>
    public interface ISearchContextHelper
    {
        SearchContextWithFieldValues GetSearchContextWithFieldValuesFromJqlString(string query);

        ISearchContext GetSearchContextFromJqlString(string query);

        SearchContextWithFieldValues GetSearchContextWithFieldValuesFromQuery(ISearchContext searchContext, IQuery query);
    }
}