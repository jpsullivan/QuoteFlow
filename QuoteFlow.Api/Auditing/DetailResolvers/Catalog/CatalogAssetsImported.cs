using Jil;

namespace QuoteFlow.Api.Auditing.DetailResolvers.Catalog
{
    public class CatalogAssetsImported : IDetailResolver
    {
        public CatalogAssetsImported()
        {
        }

        public CatalogAssetsImported(int totalAssetsImported)
        {
            TotalAssetsImported = totalAssetsImported;
        }

        /// <summary>
        /// The total number of assets that were successfully imported.
        /// </summary>
        public int TotalAssetsImported { get; set; }

        public string Serialize()
        {
            return JSON.Serialize(this);
        }
    }
}