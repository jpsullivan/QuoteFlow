namespace QuoteFlow.Models.Search.Jql.Context
{
    public class CatalogAssetTypeContext : ICatalogAssetTypeContext
    {
        private static readonly ICatalogAssetTypeContext Instance = new CatalogAssetTypeContext(AllCatalogsContext.Instance, AllAssetTypesContext.Instance);

		public static ICatalogAssetTypeContext CreateGlobalContext()
		{
			return Instance;
		}

        public CatalogAssetTypeContext(ICatalogContext catalogContext, IAssetTypeContext assetTypeContext)
		{
            CatalogContext = catalogContext;
            AssetTypeContext = AssetTypeContext;
		}

		public virtual ICatalogContext CatalogContext { get; private set; }

        public IAssetTypeContext AssetTypeContext { get; private set; }
    }
}