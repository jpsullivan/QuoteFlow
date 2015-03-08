using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Asset.Nav
{
    /// <summary>
    /// Strategy interface for creating an <see cref="AssetTable"/>.
    /// </summary>
    public abstract class AssetTableCreator
    {
        /// <summary>
        /// Validates the search.
        /// </summary>
        /// <returns>A <see cref="MessageSet"/> containing validation errors.</returns>
        public virtual IMessageSet Validate()
        {
            return new MessageSet();
        }

        /// <summary>
        /// Runs the search and creates an <see cref="AssetTable"/>.
        /// </summary>
        /// <returns>An <see cref="AssetTable"/>.</returns>
        /// <exception cref="SearchException"> </exception>
        public abstract AssetTable Create();
    }
}