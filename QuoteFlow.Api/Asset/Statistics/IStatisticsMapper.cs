using QuoteFlow.Api.Asset.Search;

namespace QuoteFlow.Api.Asset.Statistics
{
    /// <summary>
    /// Allow mapping from Lucene indexes, back to the fields that they came from.
    /// Any 'field' that implements this is capable of having a statistic calculated from it.
    /// </summary>
    public interface IStatisticsMapper<T> : ILuceneFieldSorter<T>
    {
        /// <summary>
        /// Check whether this value is valid for this particular search.  This is useful 
        /// if you do not wish to display all the values that are indexed 
        /// (eg - only show released versions).
        /// </summary>
        /// <param name="value">This is the same value that will be returned from <see cref="GetValueFromLuceneField(string)"/>.</param>
        /// <returns>True if this value is valid for this particular search.</returns>
        bool IsValidValue(T value);

        /// <summary>
        /// Check if the field is always part of an assets data. This should only return 
        /// false in the case of a custom field where the value does not have to be set for each asset.
        /// </summary>
        /// <returns> true if this mapper will always be part of an assets data </returns>
        bool FieldAlwaysPartOfAnAsset { get; }

        /// <summary>
        /// Get a suffix for the asset navigator, which allows for filtering on this value.
        /// 
        /// eg. a catalog field would return a SearchRequest object who's QueryString method will produce
        /// <code>pid=10240</code>
        /// 
        /// Note that values returned from implementations should return values that are URLEncoded.
        /// </summary>
        /// <param name="value">This is the same value that will be returned from <seealso cref="#getValueFromLuceneField(String)"/></param>
        /// <param name="searchRequest">The search request that should be used as the 
        /// base of the newly generated SearchRequest object. If this parameter is null 
        /// then the return type will also be null.</param>
        /// <returns>A SearchRequest object that will generate the correct asset navigator 
        /// url to search the correct statistics set, null otherwise.</returns>
        SearchRequest GetSearchUrlSuffix(T value, SearchRequest searchRequest);
    }
}