using System.Collections.Generic;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Configuration;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset.Nav
{
    /// <summary>
    /// Creates <see cref="AssetTableCreator"/> instances.
    ///</summary>
    public interface IAssetTableCreatorFactory
    {
        ///  <summary>
        ///  Create an <see cref="AssetTableCreator"/> for a normal (non-stable) search.
        /// </summary>
        /// <param name="user">The user performing the search.</param>
        /// <param name="configuration">The <see cref="IAssetTableService"/> configuration to use.</param>
        ///  <param name="query">The query whose results will form the table's content.</param>
        ///  <param name="returnAssetIds">Whether asset IDs should be returned.</param>
        ///  <param name="searchRequest">The search request being executed (may differ from {@code query}).</param>
        ///  <returns>An <see cref="AssetTableCreator"/>.</returns>
        ///  <exception cref="IllegalArgumentException">If no <see cref="AssetTableCreator"/> corresponds to {@code layoutKey}.</exception>
        ///  <exception cref="RuntimeException">If creating the instance fails.</exception>
        AssetTableCreator GetNormalAssetTableCreator(User user, IAssetTableServiceConfiguration configuration, IQuery query, bool returnAssetIds, SearchRequest searchRequest);

        /// <summary>
        /// Create an <see cref="AssetTableCreator"/> for a stable search.
        ///</summary>
        /// <param name="configuration">The <see cref="IAssetTableService"/> configuration to use.</param>
        /// <param name="query">The query object containing the JQL of the client state at the time of making this service call. Used to generate valid sort JQL.</param>
        /// <param name="assetIds">The IDs of the assets to render in the table.</param>
        /// <param name="searchRequest">The search request containing the id of the filter used in the initial search for the list of {@code assetIds}</param>
        /// <param name="user">The user requesting this information.</param>
        /// <returns>An <see cref="AssetTableCreator"/>.</returns>
        /// <exception cref="IllegalArgumentException">If no <see cref="AssetTableCreator"/> corresponds to {@code layoutKey}.</exception>
        /// <exception cref="RuntimeException">If creating the instance fails.</exception>
        AssetTableCreator GetStableAssetTableCreator(User user, IAssetTableServiceConfiguration configuration, IQuery query, List<int?> assetIds, SearchRequest searchRequest);
    }
}