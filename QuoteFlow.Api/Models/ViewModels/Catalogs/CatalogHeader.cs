namespace QuoteFlow.Api.Models.ViewModels.Catalogs
{
    /// <summary>
    /// A re-usable ViewModel for displaying catalog header information
    /// (via the partials/catalog/header.cshtml file).
    /// </summary>
    public class CatalogHeader
    {
        /// <summary>
        /// The user who created this catalog.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// The name of the catalog that this is for.
        /// </summary>
        public string CatalogName { get; set; }

        public CatalogHeader(User user, Catalog catalog)
        {
            User = user;
            CatalogName = catalog.Name;
        }
    }
}