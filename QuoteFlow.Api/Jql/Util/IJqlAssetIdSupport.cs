namespace QuoteFlow.Api.Jql.Util
{
    /// <summary>
    /// Provide JQL with some helper functions when dealing with Asset IDs.
    /// </summary>
    public interface IJqlAssetIdSupport
    {
        /// <summary>
        /// Determines is the passed asset id is valid for QuoteFlow. It does *NOT* determine 
        /// if the issue actually exists within QuoteFlow.
        /// </summary>
        /// <param name="assetId">The asset id to validate. Zero will be considered an invalid ID.</param>
        /// <returns>True if the passed id is valid or false otherwise. </returns>
        bool IsValidAssetId(int assetId);
    }
}